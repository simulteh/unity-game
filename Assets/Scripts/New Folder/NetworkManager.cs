using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class NetworkManager : MonoBehaviour
{
    public int NumberOfRouters = 3;
    public int NumberOfComputers = 5;

    private List<Router> routers = new List<Router>();

    void Start()
    {
        // Создание маршрутизаторов
        for (int i = 0; i < NumberOfRouters; i++)
        {
            Router router = new Router($"Router{i}"); //Создаем Router
            routers.Add(router);

            Debug.Log($"Created Router: {router.RouterId}");
        }

        // Настройка таблицы маршрутизации (пример)
        if (routers.Count >= 2)
        {
            routers[0].AddRoute("192.168.2.0/24", routers[1].RouterId, "eth0"); // Маршрут через Router1
            Debug.Log($"Added route to Router0: 192.168.2.0/24 via Router1 (eth0)");

            // Отправка пакета с Router0 на Router1
            //Packet packet = new Packet("192.168.1.10", "192.168.2.10", 5, "Hello, network!");
            //routers[0].ReceivePacket(packet);
        }
    }

    // Поиск маршрутизатора по ID
    public Router FindRouter(string routerId)
    {
        return routers.FirstOrDefault(r => r.RouterId == routerId);
    }
}
