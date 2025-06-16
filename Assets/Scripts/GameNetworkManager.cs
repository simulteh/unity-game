using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

    // Отправка сообщения в виде строки
    public bool SendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            udpClient.Send(data, data.Length, remoteEndPoint);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при отправке UDP сообщения: {e.Message}");
            return false;
        }
    }

    // Получение сообщения (блокирующий вызов)
    public string ReceiveMessage()
    {
        try
        {
            IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpClient.Receive(ref senderEndPoint);
            string message = Encoding.UTF8.GetString(data);
            return message;
        }
        catch (SocketException e)
        {
            Debug.LogError($"Ошибка при получении UDP сообщения: {e.Message}");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"Общая ошибка при получении UDP сообщения: {e.Message}");
            return null;
        }
    }
}
