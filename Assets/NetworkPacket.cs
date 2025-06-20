using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class NetworkPacket
{
    // Приватные поля
    private Dictionary<string, object> _header;
    private byte[] _data;
    private uint _checksum;

    // Публичные свойства только для чтения
    public Dictionary<string, object> Header => new Dictionary<string, object>(_header);
    public byte[] Data => (byte[])_data.Clone();
    public uint Checksum => _checksum;

    // Конструкторы
    public NetworkPacket()
    {
        _header = new Dictionary<string, object>();
        _data = new byte[0];
        _checksum = CalculateChecksum();
    }

    public NetworkPacket(Dictionary<string, object> header, byte[] data)
    {
        _header = header ?? new Dictionary<string, object>();
        _data = data ?? new byte[0];
        _checksum = CalculateChecksum();
    }
    public void UpdateHeaderField(string field, object value)
    {
        if (!_header.ContainsKey(field))
        {
            Debug.LogError($"Error: Header field '{field}' not found");
            return;
        }

        _header[field] = value;
        _checksum = CalculateChecksum();
    }

    // Получение заголовка
    public Dictionary<string, object> GetHeader()
    {
        return new Dictionary<string, object>(_header);
    }

    // Получение данных
    public byte[] GetData()
    {
        return (byte[])_data.Clone();
    }

    // Проверка контрольной суммы
    public bool VerifyChecksum()
    {
        bool isValid = _checksum == CalculateChecksum();
        if (!isValid)
        {
            Debug.LogWarning("Packet checksum verification failed - data may be corrupted");
        }
        return isValid;
    }
    // Вычисление контрольной суммы
    public uint CalculateChecksum()
    {
        // Сериализация заголовка в байты
        byte[] headerBytes = SerializeHeader(_header);

        // Объединение с данными
        byte[] combined = new byte[headerBytes.Length + _data.Length];
        Buffer.BlockCopy(headerBytes, 0, combined, 0, headerBytes.Length);
        Buffer.BlockCopy(_data, 0, combined, headerBytes.Length, _data.Length);

        // Расчет CRC32
        return Crc32.Compute(combined);
    }

    // Вспомогательный метод для сериализации заголовка
    private byte[] SerializeHeader(Dictionary<string, object> header)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            foreach (var kvp in header.OrderBy(x => x.Key))
            {
                writer.Write(kvp.Key);
                switch (kvp.Value)
                {
                    case int i: writer.Write(i); break;
                    case float f: writer.Write(f); break;
                    case string s: writer.Write(s); break;
                    case bool b: writer.Write(b); break;
                    default: writer.Write(kvp.Value.ToString()); break;
                }
            }
            return ms.ToArray();
        }
    }
    // Для сериализации в JSON
    [Serializable]
    private class PacketData
    {
        public Dictionary<string, object> Header;
        public string Data;
        public uint Checksum;
    }

    public string ToJson()
    {
        var packetData = new PacketData
        {
            Header = _header,
            Data = Convert.ToBase64String(_data),
            Checksum = _checksum
        };
        return JsonUtility.ToJson(packetData);
    }

    public static NetworkPacket FromJson(string json)
    {
        try
        {
            var packetData = JsonUtility.FromJson<PacketData>(json);
            var packet = new NetworkPacket(
                packetData.Header,
                Convert.FromBase64String(packetData.Data)
            );

            if (packet.Checksum != packetData.Checksum)
            {
                Debug.LogWarning("Checksum mismatch during deserialization");
            }

            return packet;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to deserialize packet: {e.Message}");
            return null;
        }

    }
    public static NetworkPacket GenerateTestPacket(int dataSize = 256)
    {
        var random = new System.Random();

        var header = new Dictionary<string, object>
    {
        { "TestPacket", true },
        { "RandomValue", random.Next() },
        { "Timestamp", DateTime.UtcNow.Ticks }
    };

        byte[] data = new byte[dataSize];
        random.NextBytes(data);

        return new NetworkPacket(header, data);
    }
    // Проверка валидности пакета
    public bool Validate()
    {
        // Проверка обязательных полей IP-адресов
        if (!_header.ContainsKey("SourceIP") || !_header.ContainsKey("DestinationIP"))
        {
            Debug.LogError("Missing required IP addresses in header");
            return false;
        }

        // Проверка специфичных требований для разных протоколов
        if (_header.TryGetValue("Protocol", out var protocol))
        {
            // Если protocol является строкой
            if (protocol is string protoStr)
            {
                // Для TCP-пакетов проверяем наличие SequenceNumber
                if (protoStr == "TCP" && !_header.ContainsKey("SequenceNumber"))
                {
                    Debug.LogError("TCP packets require SequenceNumber");
                    return false;
                }

                // Здесь можно добавить проверки для других протоколов
                // Например, для UDP:
                if (protoStr == "UDP" && !_header.ContainsKey("Port"))
                {
                    Debug.LogError("UDP packets require Port");
                    return false;
                }
            }
        }

        // Если все проверки пройдены
        return true;
    }
}
