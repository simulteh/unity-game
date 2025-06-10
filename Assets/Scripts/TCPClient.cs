using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

public class TcpClientScript : MonoBehaviour
{
    [Header("Client Settings")]
    public string serverIp = "127.0.0.1";
    public int serverPort = 12345;

    [Header("Status")]
    public string status = "Disconnected";
    public string lastReceivedMessage = "N/A";
    
    //[SerializeField]
    private  int _receivedMessageCount;

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _clientThread;
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private bool _isConnected;

    void Update()
    {
        while (_messageQueue.TryDequeue(out string message))
        {
            lastReceivedMessage = message;
            Debug.Log($"[Client] Received in main thread: {message}");
            _receivedMessageCount++;
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }
    
    public void ConnectToServer()
    {
        if (_isConnected)
        {
            Debug.LogWarning("[Client] Already connected or trying to connect.");
            return;
        }

        status = "Connecting...";
        Debug.Log($"[Client] Attempting to connect to {serverIp}:{serverPort}...");

        _clientThread = new Thread(ConnectThread);
        _clientThread.IsBackground = true;
        _clientThread.Start();
    }

    private void ConnectThread()
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(serverIp, serverPort);
            _stream = _client.GetStream();
            _isConnected = true;
            status = $"Connected to {serverIp}:{serverPort}";
            Debug.Log($"[Client] {status}");

            ListenForData();
        }
        catch (SocketException e)
        {
            status = $"Connection Error: {e.Message}";
            Debug.LogError($"[Client] {status}");
            _isConnected = false;
        }
        catch (Exception e)
        {
            status = $"Unexpected Error: {e.Message}";
            Debug.LogError($"[Client] {status}");
            _isConnected = false;
        }
        finally
        {
            if (!_isConnected)
            {
                status = "Disconnected";
            }
        }
    }
    public void Disconnect()
    {
        if (!_isConnected)
        {
            Debug.LogWarning("[Client] Not connected.");
            return;
        }

        _isConnected = false;
        status = "Disconnecting...";
        Debug.Log("[Client] Disconnecting...");

        if (_stream != null) _stream.Close();
        if (_client != null) _client.Close();

        if (_clientThread != null && _clientThread.IsAlive)
        {
            _clientThread.Join(100);
            if (_clientThread.IsAlive)
            {
                _clientThread.Interrupt();
            }
        }

        status = "Disconnected";
        Debug.Log("[Client] Disconnected.");
    }

    private void ListenForData()
    {
        byte[] bytes = new byte[_client.ReceiveBufferSize];
        while (_isConnected && _client.Connected)
        {
            try
            {
                int bytesRead = _stream.Read(bytes, 0, bytes.Length);
                if (bytesRead > 0)
                {
                    string dataReceived = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                    _messageQueue.Enqueue($"[Server] {dataReceived}");
                }
                else if (bytesRead == 0)
                {
                    Debug.Log("[Client] Server disconnected gracefully (Read returned 0 bytes).");
                    break;
                }
            }
            catch (ObjectDisposedException)
            {
                Debug.Log("[Client] Stream/socket disposed (expected on disconnect).");
                break;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Client] Error receiving data: {e.Message}");
                break; 
            }
        }
        Disconnect();
    }
    public void SendMessageToServer(string message)
    {
        if (!_isConnected || _stream == null)
        {
            Debug.LogWarning("[Client] Not connected to server. Cannot send data.");
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(message);

        try
        {
            _stream.Write(data, 0, data.Length);
            Debug.Log($"[Client] Sent: {message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Client] Error sending message: {e.Message}");
            Disconnect();
        }
    }
    
    
    
    // void OnGUI()
    // {
    //     GUI.skin.label.fontSize = 18;
    //     GUI.skin.button.fontSize = 18;
    //     GUI.skin.textField.fontSize = 18;
    //
    //     float startY = 10;
    //     float height = 35;
    //     float spacing = 5;
    //     float offsetX = 450;
    //
    //     GUI.Label(new Rect(offsetX + 10, startY, 400, height), $"Client Status: {status}");
    //     startY += height + spacing;
    //     GUI.Label(new Rect(offsetX + 10, startY, 600, height), $"Last Client Msg: {lastReceivedMessage}");
    //     startY += height + spacing;
    //     GUI.Label(new Rect(offsetX + 10, startY, 600, height), $"Total Msg Count: {_receivedMessageCount}");
    //     startY += height + spacing;
    //     
    //
    //     if (GUI.Button(new Rect(offsetX + 10, startY, 150, height), "Connect"))
    //     {
    //         ConnectToServer();
    //     }
    //     if (GUI.Button(new Rect(offsetX + 170, startY, 150, height), "Disconnect"))
    //     {
    //         Disconnect();
    //     }
    //     startY += height + spacing;
    //
    //     GUI.Label(new Rect(offsetX + 10, startY, 200, height), "Send to Server:");
    //     startY += height + spacing;
    //     
    //
    //     if (GUI.Button(new Rect(offsetX + 10, startY, 150, height), "Hello Server"))
    //     {
    //         SendMessageToServer("Hello from Client!");
    //     }
    //     if (GUI.Button(new Rect(offsetX + 170, startY, 150, height), "Client Ping"))
    //     {
    //         SendMessageToServer("Client Ping!");
    //     }
    // }
}