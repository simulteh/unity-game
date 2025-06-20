using UnityEngine;
using System.Collections.Generic;


public class NetworkPacketTest : MonoBehaviour
{
    void Start()
    {
        TestBasicFunctionality();
        TestJsonSerialization();
        TestChecksumVerification();
    }

    void TestBasicFunctionality()
    {
        Debug.Log("=== Testing Basic Functionality ===");

        var header = new Dictionary<string, object>
        {
            { "SourceIP", "192.168.1.1" },
            { "DestinationIP", "192.168.1.2" },
            { "Protocol", "TCP" },
            { "TTL", 64 }
        };

        byte[] data = System.Text.Encoding.UTF8.GetBytes("Test packet data");
        var packet = new NetworkPacket(header, data);

        // Проверка заголовка
        foreach (var field in packet.GetHeader())
        {
            Debug.Log($"{field.Key}: {field.Value}");
        }

        // Проверка данных
        Debug.Log($"Data: {System.Text.Encoding.UTF8.GetString(packet.GetData())}");

        // Проверка контрольной суммы
        Debug.Log($"Checksum valid: {packet.VerifyChecksum()}");
    }

    void TestJsonSerialization()
    {
        Debug.Log("=== Testing JSON Serialization ===");

        var header = new Dictionary<string, object>
        {
            { "Test", true },
            { "RandomValue", 42 },
            { "Timestamp", System.DateTime.UtcNow.Ticks }
        };

        byte[] data = new byte[100];
        new System.Random().NextBytes(data);

        var originalPacket = new NetworkPacket(header, data);
        string json = originalPacket.ToJson();
        Debug.Log($"Serialized JSON: {json}");

        var deserializedPacket = NetworkPacket.FromJson(json);
        Debug.Log($"Deserialized checksum valid: {deserializedPacket.VerifyChecksum()}");
    }

    void TestChecksumVerification()
    {
        Debug.Log("=== Testing Checksum Verification ===");

        var packet = new NetworkPacket(
            new Dictionary<string, object> { { "Test", "Checksum" } },
            System.Text.Encoding.UTF8.GetBytes("Test data")
        );

        Debug.Log($"Original checksum valid: {packet.VerifyChecksum()}");

        // Намеренная порча данных
        var corruptedData = packet.GetData();
        corruptedData[0] ^= 0xFF; // Инвертируем первый бит
        var corruptedPacket = new NetworkPacket(packet.GetHeader(), corruptedData);

        Debug.Log($"Corrupted checksum valid: {corruptedPacket.VerifyChecksum()}");
    }
}