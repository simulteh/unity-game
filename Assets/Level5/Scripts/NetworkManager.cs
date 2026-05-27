using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Устройства")]
    public Switch switchDevice;
    public NetworkDevice pc1;
    public NetworkDevice pc2;
    public NetworkDevice pc3;

    [Header("Назначение портов")]
    public int pc1Port = 1;
    public int pc2Port = 2;
    public int pc3Port = 3;

    [Header("Trunk порты (если нужно)")]
    public bool pc1IsTrunk = false;
    public bool pc2IsTrunk = false;
    public bool pc3IsTrunk = false;

    void Start()
    {
        if (switchDevice == null)
        {
            Debug.LogError("[NetworkManager] Коммутатор не назначен!");
            return;
        }

        if (pc1 == null) Debug.LogWarning("[NetworkManager] PC1 не назначен");
        if (pc2 == null) Debug.LogWarning("[NetworkManager] PC2 не назначен");
        if (pc3 == null) Debug.LogWarning("[NetworkManager] PC3 не назначен");

        if (pc1 != null)
            switchDevice.RegisterDevice(pc1Port, pc1, pc1IsTrunk);

        if (pc2 != null)
            switchDevice.RegisterDevice(pc2Port, pc2, pc2IsTrunk);

        if (pc3 != null)
            switchDevice.RegisterDevice(pc3Port, pc3, pc3IsTrunk);

        Debug.Log("[NetworkManager] Все устройства зарегистрированы!");
        Debug.Log($"[NetworkManager] На коммутаторе {switchDevice.connectedDevices.Count} устройств");
    }

    [ContextMenu("Send Test Frame from PC1 to PC2")]
    public void SendTestFrame_PC1_to_PC2()
    {
        if (pc1 == null || pc2 == null || switchDevice == null)
        {
            Debug.LogError("[NetworkManager] Нет ссылок для тестового кадра!");
            return;
        }

        var testFrame = new NetworkFrame(
            pc1.MacAddress,
            pc2.MacAddress,
            pc1.VlanId,
            "Привет от PC1!"
        );

        Debug.Log($"=== ТЕСТ: {pc1.Name} → {pc2.Name} (одинаковый VLAN) ===");
        pc1.SendFrame(testFrame, switchDevice);
    }

    [ContextMenu("Send Test Frame from PC1 to PC3")]
    public void SendTestFrame_PC1_to_PC3()
    {
        if (pc1 == null || pc3 == null || switchDevice == null)
        {
            Debug.LogError("[NetworkManager] Нет ссылок для тестового кадра!");
            return;
        }

        var testFrame = new NetworkFrame(
            pc1.MacAddress,
            pc3.MacAddress,
            pc1.VlanId,
            "Привет от PC1 для PC3 (другой VLAN)!"
        );

        Debug.Log($"=== ТЕСТ: {pc1.Name} → {pc3.Name} (разные VLAN) ===");
        pc1.SendFrame(testFrame, switchDevice);
    }
}
