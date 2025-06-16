using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    private UDPCommunicator udp;

    void Start()
    {
        // Добавляем компонент UDPCommunicator к этому объекту
        udp = gameObject.AddComponent<UDPCommunicator>();

        // Инициализируем с локальным IP и портом 9050 (можно заменить на нужные)
        udp.Initialize("127.0.0.1", 9050);

        // Подписываемся на событие получения сообщения
        udp.MessageReceived += OnMessageReceived;

        // Запускаем корутину для приема сообщений
        StartCoroutine(udp.ReceiveMessagesCoroutine());
    }

    void OnDestroy()
    {
        // Закрываем UDP клиент при уничтожении объекта
        udp.Close();
    }

    // Обработчик входящих сообщений
    private void OnMessageReceived(string message)
    {
        Debug.Log("Получено UDP сообщение: " + message);
        // Здесь можно добавить обработку игрового события или состояния
    }
}
