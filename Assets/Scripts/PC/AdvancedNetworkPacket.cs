// AdvancedNetworkPacket.cs
using Unity.Netcode;
using UnityEngine;

public class AdvancedNetworkPacket : NetworkBehaviour
{
    public struct PacketData : INetworkSerializable
    {
        public FixedString64Bytes SourceIP;
        public FixedString64Bytes DestinationIP;
        public FixedString512Bytes Payload;
        public short TTL;
        public bool IsEncrypted;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref SourceIP);
            serializer.SerializeValue(ref DestinationIP);
            serializer.SerializeValue(ref Payload);
            serializer.SerializeValue(ref TTL);
            serializer.SerializeValue(ref IsEncrypted);
        }
    }

    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject packetVisual;
    
    private NetworkVariable<PacketData> packetInfo = new();
    private Vector3 targetPosition;

    public void Initialize(PacketData data, Vector3 target)
    {
        packetInfo.Value = data;
        targetPosition = target;
    }

    void Update()
    {
        if (!IsServer) return;
        
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            OnReachDestination();
        }
    }

    private void OnReachDestination()
    {
        // Логика обработки при получении
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(
            OwnerClientId, out var client))
        {
            var computer = client.PlayerObject.GetComponent<NetworkNode>();
            if (computer != null)
            {
                computer.ReceivePacket(packetInfo.Value);
            }
        }
        
        DestroyPacketClientRpc();
    }

    [ClientRpc]
    private void DestroyPacketClientRpc()
    {
        Destroy(gameObject);
    }
}