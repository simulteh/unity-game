using UnityEngine;
using System.Collections.Generic;

public class NetworkScenarioManager : MonoBehaviour
{
    public List<NetworkDevice> devices;
    public AdminTabletUI tabletUI;
    public ManualFD notebook;

    private void Start()
    {
        LoadScenario("CafeNetwork");
        notebook.ShowNote("Папа, не забудь проверить IP-адрес на кофемашине — она опять не выходит в интернет! ❤️");
    }

    public void LoadScenario(string scenarioName)
    {
        if (scenarioName == "CafeNetwork")
        {
            devices = new List<NetworkDevice>
            {
                new NetworkDevice("WiFiRouter", "192.168.1.1"),
                new NetworkDevice("CoffeeMachine", "192.168.1.100"),
                new NetworkDevice("POS Terminal", "192.168.1.101")
            };

            tabletUI.LoadDevices(devices);
        }
    }

    public void ValidateConfiguration()
    {
        bool allConnected = true;
        foreach (var device in devices)
        {
            if (!device.IsConfiguredProperly())
            {
                allConnected = false;
                notebook.ShowNote($"Папа, кажется, ты забыл настроить {device.Name}...");
            }
        }

        if (allConnected)
        {
            notebook.ShowNote("Круто, всё работает! Папа, ты настоящий сетевой маг! ✨");
        }
    }
}
