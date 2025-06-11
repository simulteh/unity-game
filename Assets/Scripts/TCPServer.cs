using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
public class TcpServer : MonoBehaviour
{
    [Header("Server Settings")]
    private int _port = 12345; // Port for listening to incoming connections
    private string _serverIpAddress = "127.0.0.1"; // Server IP address

    [Header("Status")]
    private string _status = "Idle";
    private string _lastReceivedMessage = "N/A";
    private int _connectedClientsCount;
    
    //[SerializeField]
    private int _receivedMessageCount;

    private TcpListener _tcpListener;
    private Thread _listenThread;
    private List<ClientHandler> _connectedClients = new List<ClientHandler>();
    private ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private bool _isListening; 
    
    void Start()
     {
         StartServer();
     }

    void Update()
    {
        while (_messageQueue.TryDequeue(out string message))
        {
            _lastReceivedMessage = message;
            Debug.Log($"[Server] Received in main thread: {message}");
            _receivedMessageCount++;

        }

        _connectedClientsCount = _connectedClients.Count;
    }

    void OnApplicationQuit()
    {
        StopServer(); 
    }

    public void StartServer()
    {
        if (_isListening)
        {
            Debug.LogWarning("[Server] Server is already running.");
            return;
        }

        try
        {
            IPAddress ipAddress = IPAddress.Parse(_serverIpAddress);
            _tcpListener = new TcpListener(ipAddress, _port);
            _tcpListener.Start();
            _isListening = true;
            _status = $"Listening on {_serverIpAddress}:{_port}";
            Debug.Log($"[Server] {_status}");
            
            _listenThread = new Thread(ListenForClients);
            _listenThread.IsBackground = true;
            _listenThread.Start();
        }
        catch (Exception e)
        {
            _status = $"Error starting server: {e.Message}";
            Debug.LogError($"[Server] {_status}");
            _isListening = false;
        }
    }
    public void StopServer()
    {
        if (!_isListening)
        {
            Debug.LogWarning("[Server] Server is not running.");
            return;
        }

        _isListening = false;
        _status = "Stopping...";
        Debug.Log("[Server] Stopping server...");

        if (_tcpListener != null)
        {
            _tcpListener.Stop();
        }
        
        foreach (ClientHandler client in new List<ClientHandler>(_connectedClients))
        {
            client.Stop();
        }
        _connectedClients.Clear();
        
        if (_listenThread != null && _listenThread.IsAlive)
        {
            _listenThread.Join(); 
        }

        _status = "Stopped";
        Debug.Log("[Server] Server stopped.");
    }

    private void ListenForClients()
    {
        while (_isListening) 
        {
            try
            {
                TcpClient client = _tcpListener.AcceptTcpClient();
                Debug.Log($"[Server] Client connected: {client.Client.RemoteEndPoint}");
                
                ClientHandler clientHandler = new ClientHandler(client, _messageQueue, RemoveClient);
                _connectedClients.Add(clientHandler);
                clientHandler.Start();
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.Interrupted || e.SocketErrorCode == SocketError.OperationAborted)
                {
                    Debug.Log("[Server] Listener stopped (expected on server shutdown).");
                }
                else if (_isListening)
                {
                    Debug.LogError($"[Server] Socket error in ListenForClients: {e.Message}");
                }
                break;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Server] Unexpected error in ListenForClients: {e.Message}");
                break;
            }
        }
    }

    private void RemoveClient(ClientHandler handler)
    {
        if (_connectedClients.Contains(handler))
        {
            _connectedClients.Remove(handler);
            Debug.Log($"[Server] Client disconnected: {handler.ClientEndPoint}. Clients remaining: {_connectedClients.Count}");
        }
    }
    
    public void SendToAllClients(string message)
    {
        if (_connectedClients.Count == 0)
        {
            Debug.LogWarning("[Server] No clients connected to send data.");
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(message);
        foreach (ClientHandler client in new List<ClientHandler>(_connectedClients))
        {
            client.SendData(data);
        }
    }
    private class ClientHandler
    {
        public TcpClient client;
        private NetworkStream _stream;
        private Thread _clientThread;
        private ConcurrentQueue<string> _serverMessageQueue;
        private Action<ClientHandler> _removeClientCallback;
        private bool _isRunning;

        public string ClientEndPoint => client.Client.RemoteEndPoint.ToString();

        public ClientHandler(TcpClient tcpClient, ConcurrentQueue<string> msgQueue, Action<ClientHandler> removeCallback)
        {
            client = tcpClient;
            _serverMessageQueue = msgQueue;
            _removeClientCallback = removeCallback;
        }

        public void Start()
        {
            _isRunning = true;
            _stream = client.GetStream();
            _clientThread = new Thread(HandleClientComm);
            _clientThread.IsBackground = true;
            _clientThread.Start();
        }

        public void Stop()
        {
            if (!_isRunning) return;
            _isRunning = false;
            Debug.Log($"[ClientHandler] Stopping client: {ClientEndPoint}");

            if (_stream != null) _stream.Close();
            if (client != null) client.Close();

            if (_clientThread != null && _clientThread.IsAlive)
            {
                _clientThread.Join(100);
                if (_clientThread.IsAlive)
                {
                    _clientThread.Interrupt();
                }
            }
        }

        private void HandleClientComm()
        {
            byte[] bytes = new byte[client.ReceiveBufferSize];
            while (_isRunning && client.Connected)
            {
                try
                {
                    int bytesRead = _stream.Read(bytes, 0, bytes.Length);
                    if (bytesRead > 0)
                    {
                        string dataReceived = Encoding.UTF8.GetString(bytes, 0, bytesRead);
                        _serverMessageQueue.Enqueue($"[{ClientEndPoint}] {dataReceived}");
                    }
                    else if (bytesRead == 0)
                    {
                        Debug.Log($"[ClientHandler] Client {ClientEndPoint} disconnected gracefully.");
                        break;
                    }
                }
                catch (ObjectDisposedException)
                {
                    Debug.Log($"[ClientHandler] Client {ClientEndPoint} stream/socket disposed (expected).");
                    break;
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ClientHandler] Error handling client {ClientEndPoint}: {e.Message}");
                    break;
                }
            }
            Stop();
            _removeClientCallback?.Invoke(this);
        }
        
        public void SendData(byte[] data)
        {
            if (client.Connected && _isRunning && _stream != null)
            {
                try
                {
                    _stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ClientHandler] Error sending data to {ClientEndPoint}: {e.Message}");
                    Stop();
                    _removeClientCallback?.Invoke(this);
                }
            }
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
    //
    //     GUI.Label(new Rect(10, startY, 400, height), $"Server Status: {status}");
    //     startY += height + spacing;
    //     GUI.Label(new Rect(10, startY, 400, height), $"Clients Connected: {connectedClientsCount}");
    //     startY += height + spacing;
    //     GUI.Label(new Rect(10, startY, 600, height), $"Last Server Msg: {lastReceivedMessage}");
    //     startY += height + spacing;
    //     GUI.Label(new Rect(10, startY, 600, height), $"Total Msg Count: {_receivedMessageCount}");
    //     startY += height + spacing;
    //
    //     if (GUI.Button(new Rect(10, startY, 150, height), "Start Server"))
    //     {
    //         StartServer();
    //     }
    //     if (GUI.Button(new Rect(170, startY, 150, height), "Stop Server"))
    //     {
    //         StopServer();
    //     }
    //     startY += height + spacing;
    //
    //     GUI.Label(new Rect(10, startY, 200, height), "Send to All Clients:");
    //     startY += height + spacing;
    //
    //     if (GUI.Button(new Rect(10, startY, 150, height), "Hello All"))
    //     {
    //         SendToAllClients("Hello everyone from Server!");
    //     }
    //     if (GUI.Button(new Rect(170, startY, 150, height), "Ping All"))
    //     {
    //         SendToAllClients("Ping!");
    //     }
    //     startY += height + spacing;
    // }
}
