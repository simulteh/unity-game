using UnityEngine;

public class AdvancedNetworkPacket : MonoBehaviour
{
    public struct PacketData
    {
        public string SourceIP;
        public string DestinationIP;
        public string Payload;
        public short TTL;
        public bool IsEncrypted;

        // Для сериализации в данном случае просто выведем данные для симуляции
        public void DisplayInfo()
        {
            Debug.Log($"Source IP: {SourceIP}");
            Debug.Log($"Destination IP: {DestinationIP}");
            Debug.Log($"Payload: {Payload}");
            Debug.Log($"TTL: {TTL}");
            Debug.Log($"IsEncrypted: {IsEncrypted}");
        }
    }

    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject packetVisual;

    private PacketData packetInfo;
    private Vector3 targetPosition;
    private bool isInitialized = false;

    public void Initialize(PacketData data, Vector3 target)
    {
        packetInfo = data;
        targetPosition = target;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

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
        Debug.Log("Packet reached its destination.");
        ReceivePacket(packetInfo);
        Destroy(gameObject);
    }

    // Симулируем получение пакета
    private void ReceivePacket(PacketData packet)
    {
        Debug.Log("Received packet:");
        packet.DisplayInfo();
    }
}
