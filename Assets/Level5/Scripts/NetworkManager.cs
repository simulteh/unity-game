using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Devices")]
    public Switch switchDevice;      // ← теперь это Switch, а не NetworkDevice
    public NetworkDevice pc1;
    public NetworkDevice pc2;
    public NetworkDevice pc3;

    [Header("Port Assignments")]
    public int pc1Port = 1;
    public int pc2Port = 2;
    public int pc3Port = 3;

    [Header("Trunk Ports (если нужно)")]
    public bool pc1IsTrunk = false;
    public bool pc2IsTrunk = false;
    public bool pc3IsTrunk = false;

    void Start()
    {
        if (switchDevice == null)
        {
            Debug.LogError("[NetworkManager] Switch not assigned! Please drag the Switch object into the 'Switch Device' field in Inspector.");
            return;
        }

        // Проверяем, что устройства назначены
        if (pc1 == null) Debug.LogWarning("[NetworkManager] PC1 not assigned");
        if (pc2 == null) Debug.LogWarning("[NetworkManager] PC2 not assigned");
        if (pc3 == null) Debug.LogWarning("[NetworkManager] PC3 not assigned");

        // Регистрируем каждое устройство на коммутаторе
        if (pc1 != null)
            switchDevice.RegisterDevice(pc1Port, pc1, pc1IsTrunk);

        if (pc2 != null)
            switchDevice.RegisterDevice(pc2Port, pc2, pc2IsTrunk);

        if (pc3 != null)
            switchDevice.RegisterDevice(pc3Port, pc3, pc3IsTrunk);

        Debug.Log("[NetworkManager] All devices registered!");

        // Выводим список зарегистрированных устройств
        Debug.Log($"[NetworkManager] Switch has {switchDevice.connectedDevices.Count} connected devices");
    }

    // Тестовый метод для отправки кадра от PC1 к PC2
    [ContextMenu("Send Test Frame from PC1 to PC2")]
    public void SendTestFrame_PC1_to_PC2()
    {
        if (pc1 == null || pc2 == null || switchDevice == null)
        {
            Debug.LogError("[NetworkManager] Missing references for test frame!");
            return;
        }

        var testFrame = new NetworkFrame(
            pc1.MacAddress,
            pc2.MacAddress,
            pc1.VlanId,
            "Hello from PC1!"
        );

        Debug.Log($"=== SENDING TEST FRAME: {pc1.Name} → {pc2.Name} ===");
        pc1.SendFrame(testFrame, switchDevice);
    }

    // Тестовый метод для отправки кадра от PC1 к PC3 (разные VLAN)
    [ContextMenu("Send Test Frame from PC1 to PC3")]
    public void SendTestFrame_PC1_to_PC3()
    {
        if (pc1 == null || pc3 == null || switchDevice == null)
        {
            Debug.LogError("[NetworkManager] Missing references for test frame!");
            return;
        }

        var testFrame = new NetworkFrame(
            pc1.MacAddress,
            pc3.MacAddress,
            pc1.VlanId,
            "Hello from PC1 to PC3 (different VLAN)!"
        );

        Debug.Log($"=== SENDING TEST FRAME: {pc1.Name} → {pc3.Name} (different VLAN) ===");
        pc1.SendFrame(testFrame, switchDevice);
    }
}