using System; // Добавлено для Convert
using System.Linq; // Добавлено для LINQ-методов
using UnityEngine; // Добавлено для Debug

public class MACAddress
{
    private readonly byte[] _addressBytes = new byte[6]; // Хранит 6 байт адреса
    // Конструктор из массива байт
    public MACAddress(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 6)
            throw new ArgumentException("MAC must be 6 bytes");
        Buffer.BlockCopy(bytes, 0, _addressBytes, 0, 6);
    }

    public static MACAddress Parse(string address)
    {
        try
        {
            var parts = address.Split(':', '-');
            if (parts.Length != 6)
                throw new FormatException("Invalid MAC format");

            var bytes = parts.Select(p => Convert.ToByte(p, 16)).ToArray();
            return new MACAddress(bytes);
        }
        catch (Exception ex)
        {
            Debug.LogError($"MAC parse error: {ex.Message}");
            throw;
        }
    }

    public byte[] ToByteArray() => (byte[])_addressBytes.Clone();

    internal bool IsBroadcast()
    {
        throw new NotImplementedException();
    }
}
