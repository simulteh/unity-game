using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Router : NetworkNode
{
    public string RouterId { get; private set; }

    // Таблица маршрутизации: Destination Network -> Next Hop Router ID
    public Dictionary<string, string> RoutingTable { get; private set; } = new Dictionary<string, string>();

    // Список интерфейсов (теперь храним объекты Interface)
    public List<Interface> Interfaces { get; private set; } = new List<Interface>();

    public string DefaultGateway { get; private set; }

    public Router(string routerId)
    {
        RouterId = routerId;
    }

    // Метод для добавления маршрута (Destination Network, Next Hop Router ID, Interface ID)
    public void AddRoute(string destinationNetwork, string nextHopRouterId, string interfaceId)
    {
        // Проверка, существует ли указанный интерфейс
        if (!Interfaces.Any(i => i.InterfaceId == interfaceId))
        {
            Debug.LogError($"Router {RouterId}: Interface {interfaceId} not found.");
            return; // Прерываем добавление маршрута
        }

        RoutingTable[destinationNetwork] = nextHopRouterId;
        Debug.Log($"Router {RouterId}: Added route to {destinationNetwork} via {nextHopRouterId} (Interface: {interfaceId})");
    }

    // Метод для добавления нового интерфейса
    public void AddInterface(Interface newInterface)
    {
        Interfaces.Add(newInterface);
        Debug.Log($"Router {RouterId}: Added interface {newInterface}");
    }

    public void RemoveRoute(string destinationNetwork)
    {
        RoutingTable.Remove(destinationNetwork);
    }

    public void UpdateRoute(string destinationNetwork, string newNextHop)
    {
        if (RoutingTable.ContainsKey(destinationNetwork))
        {
            RoutingTable[destinationNetwork] = newNextHop;
        }
        else
        {
            Debug.LogError($"Route for {destinationNetwork} does not exist.");
        }
    }

    public void SetDefaultGateway(string gatewayIp)
    {
        DefaultGateway = gatewayIp;
    }

    // **Изменен: Теперь принимает экземпляр Packet**
    public void ReceivePacket(Packet packet)
    {
        Debug.Log($"Router {RouterId} received packet from {packet.SourceAddress} to {packet.DestinationAddress}");

        // TTL check
        if (packet.TTL <= 0)
        {
            Debug.LogWarning($"Router {RouterId}: Packet TTL expired. Dropping packet.");
            return; // Отбросить пакет
        }        packet.DecrementTTL(); // Уменьшаем TTL

        // Поиск маршрута в таблице
        if (RoutingTable.ContainsKey(packet.DestinationAddress))
        {
            string nextHopRouterId = RoutingTable[packet.DestinationAddress]; // Получаем ID следующего маршрутизатора
            ForwardPacket(packet, nextHopRouterId); // Отправляем пакет дальше
        }
        else
        {
            // Если маршрут не найден, отправляем на шлюз по умолчанию (если он установлен)
            if (!string.IsNullOrEmpty(DefaultGateway))
            {
                Debug.Log($"Router {RouterId}: No route found. Sending to default gateway {DefaultGateway}");
                ForwardPacket(packet, DefaultGateway);
            }
            else
            {
                Debug.LogError($"Router {RouterId}: No route found and no default gateway set. Dropping packet.");
                HandleRoutingError(packet, "No route and no default gateway"); // Обработка ошибки маршрутизации
            }
        }
    }

    // **Изменен: Теперь принимает Router ID следующего хопа**
    private void ForwardPacket(Packet packet, string nextHopRouterId)
    {
        Debug.Log($"Router {RouterId} forwarding packet to {nextHopRouterId}");

        // **Получаем экземпляр Router по его ID**
        Router nextRouter = FindRouterInNetwork(nextHopRouterId);

        if (nextRouter != null)
        {
            // Отправляем пакет следующему маршрутизатору
            nextRouter.ReceivePacket(packet);
        }
        else
        {
            Debug.LogError($"Router {RouterId}: Next hop router {nextHopRouterId} not found.");
            HandleRoutingError(packet, $"Next hop router {nextHopRouterId} not found");
        }
    }

    // Обработка ошибок маршрутизации
    private void HandleRoutingError(Packet packet, string errorMessage)
    {
        Debug.LogError($"Routing Error: {errorMessage}");
        // Можно имитировать отправку ICMP-сообщения "Destination Unreachable"
        // Отправителю пакета, но это потребует более сложной реализации
        Debug.Log($"Simulating ICMP Destination Unreachable for packet from {packet.SourceAddress} to {packet.DestinationAddress}");
    }


      public override string ToString()
    {
        return $"Router(router_id='{RouterId}')";
    }
    public override string Repr()
    {
      return $"Router(router_id='{RouterId}')";
    }
     // **Поиск маршрутизатора в сети по ID (ID маршрутизатора, а не IP)**
    private Router FindRouterInNetwork(string routerId)
    {
            NetworkManager networkManager = FindFirstObjectByType<NetworkManager>();
        if (networkManager != null)
        {
            return networkManager.FindRouter(routerId);
        }

        Debug.LogError("NetworkManager not found!");
        return null;
    }
}
