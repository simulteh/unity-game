public class GameNetworkManager : MonoBehaviour
{
    private UDPCommunicator udp;
    void Start()
    {
        udp = gameObject.AddComponent<UDPCommunicator>();
        udp.Initialize("127.0.0.1", 9050); // Например, локальный адрес и порт
        udp.MessageReceived += OnMessageReceived;

        StartCoroutine(udp.ReceiveMessagesCoroutine());
    }
    void OnDestroy()
    {
        udp.Close();
    }
    private void OnMessageReceived(string message)
    {
        Debug.Log("Получено UDP сообщение: " + message);
    }
}
