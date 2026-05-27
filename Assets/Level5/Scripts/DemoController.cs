using UnityEngine;
using System.Collections;

public class DemoController : MonoBehaviour
{
    [Header("Ссылки")]
    public NetworkManager networkManager;
    public TabletInfoPanel tabletPanel;

    [Header("Авто-демо")]
    public bool runAutoDemo = false;
    public float stepDelay = 2f;

    private void Start()
    {
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManager>();

        Debug.Log("=== КОММУТАТОР + VLAN ===");
        Debug.Log("Устройства зарегистрированы. Тесты через контекстное меню NetworkManager:");
        Debug.Log("  - Send Test Frame from PC1 to PC2 (одинаковый VLAN 10)");
        Debug.Log("  - Send Test Frame from PC1 to PC3 (разные VLAN 10 → 20)");

        if (runAutoDemo)
            StartCoroutine(AutoDemo());

        var sw = networkManager?.switchDevice;
        if (sw != null)
        {
            Debug.Log($"Портов коммутатора: {sw.ports.Count}");
            foreach (var p in sw.ports)
                Debug.Log($"  Порт {p.PortNumber}: {p.ConnectedDevice?.Name} ({p.Mode}, VLAN {p.AccessVlan})");
        }
    }

    private IEnumerator AutoDemo()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("=== ДЕМО: PC1 → PC2 (VLAN 10 — успех) ===");
        networkManager.SendTestFrame_PC1_to_PC2();
        yield return new WaitForSeconds(stepDelay);

        Debug.Log("=== ДЕМО: PC1 → PC3 (VLAN 10 → 20 — блокировка) ===");
        networkManager.SendTestFrame_PC1_to_PC3();
        yield return new WaitForSeconds(stepDelay);

        Debug.Log("=== ДЕМО ЗАВЕРШЕНО ===");
    }
}
