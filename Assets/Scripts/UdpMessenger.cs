using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPCommunicator
{
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    private byte[] receiveBuffer;
    private const int BufferSize = 1024;
    private string ipAddress;
    private int port;
    // Инициализация клиента
    public void Initialize(string ipAddress, int port)
    {
        this.ipAddress = ipAddress;
        this.port = port;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

        udpClient = new UdpClient();
        receiveBuffer = new byte[BufferSize];
    }
    // Очистка ресурсов
    public void Close()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }
    }
}
