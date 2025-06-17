// EnhancedPlayerComputer.cs
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class EnhancedPlayerComputer : NetworkBehaviour
{
    [Header("Components")]
    [SerializeField] private TMP_InputField terminalInput;
    [SerializeField] private GameObject packetPrefab;
    [SerializeField] private Transform packetSpawnPoint;
    
    [Header("Network Settings")]
    [SyncVar] private string ipAddress = "192.168.1.100";
    [SyncVar] private string subnetMask = "255.255.255.0";
    
    private void Start()
    {
        if (!IsOwner) return;
        
        terminalInput.onSubmit.AddListener(ProcessCommand);
        SetupNetworkInterface();
    }

    private void SetupNetworkInterface()
    {
        // Инициализация сетевых настроек
        Debug.Log($"Network interface initialized. IP: {ipAddress}, Subnet: {subnetMask}");
    }

    private void ProcessCommand(string command)
    {
        string[] args = command.Split(' ');
        
        if (args.Length == 0) return;

        switch (args[0].ToLower())
        {
            case "ping":
                if (args.Length > 1) SendPing(args[1]);
                break;
                
            case "send":
                if (args.Length > 2) SendDataPacket(args[1], string.Join(" ", args, 2, args.Length - 2));
                break;
                
            case "tracert":
                if (args.Length > 1) StartTraceroute(args[1]);
                break;
                
            case "ipconfig":
                DisplayNetworkInfo();
                break;
        }
    }

    private void SendPing(string destination)
    {
        var packetData = new AdvancedNetworkPacket.PacketData
        {
            SourceIP = ipAddress,
            DestinationIP = destination,
            Payload = "PING_REQUEST",
            TTL = 64,
            IsEncrypted = false
        };
        
        SpawnPacket(packetData, FindNodePosition(destination));
    }

    private void SendDataPacket(string destination, string data)
    {
        var packetData = new AdvancedNetworkPacket.PacketData
        {
            SourceIP = ipAddress,
            DestinationIP = destination,
            Payload = data,
            TTL = 64,
            IsEncrypted = false
        };
        
        SpawnPacket(packetData, FindNodePosition(destination));
    }

    private Vector3 FindNodePosition(string ip)
    {
        // Поиск позиции целевого узла в сети
        foreach (var node in FindObjectsOfType<NetworkNode>())
        {
            if (node.IPAddress == ip)
            {
                return node.transform.position;
            }
        }
        return Vector3.zero;
    }

    private void SpawnPacket(AdvancedNetworkPacket.PacketData data, Vector3 targetPos)
    {
        var packetObj = Instantiate(packetPrefab, packetSpawnPoint.position, Quaternion.identity);
        var packet = packetObj.GetComponent<AdvancedNetworkPacket>();
        
        packet.Initialize(data, targetPos);
        packet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
}