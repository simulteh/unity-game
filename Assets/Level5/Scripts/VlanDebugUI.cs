using UnityEngine;

public class VlanDebugUI : MonoBehaviour
{
    public NetworkManager networkManager;

    public KeyCode toggleKey = KeyCode.F1;
    public Color sameVlanColor = Color.green;
    public Color diffVlanColor = Color.red;

    private bool showDebug = true;
    private GUIStyle labelStyle;
    private GUIStyle titleStyle;
    private bool stylesInitialized;

    private void Start()
    {
        if (networkManager == null)
            networkManager = FindObjectOfType<NetworkManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            showDebug = !showDebug;
    }

    private void OnGUI()
    {
        if (!showDebug) return;

        if (!stylesInitialized)
        {
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 14
            };
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            stylesInitialized = true;
        }

        var sw = networkManager?.switchDevice;
        if (sw == null) return;

        float x = 10;
        float y = 10;
        float w = 480;
        float lineH = 22;
        float pad = 5;

        GUI.Box(new Rect(x, y, w, 34 + sw.connectedDevices.Count * lineH + 6 * lineH + 30), "Switch / VLAN Status");

        y += 25;

        GUI.Label(new Rect(x + pad, y, w, lineH), $"Подключено устройств: {sw.connectedDevices.Count}", labelStyle);
        y += lineH;

        GUI.Label(new Rect(x + pad, y, w, lineH), $"Записей в MAC-таблице: {sw.macTable.Count}", labelStyle);
        y += lineH + pad;

        GUI.Label(new Rect(x + pad, y, w, lineH), "--- Устройства ---", titleStyle);
        y += lineH;

        foreach (var device in sw.connectedDevices)
        {
            var port = sw.ports.Find(p => p.ConnectedDevice == device);
            string vlanInfo = device.VlanId == 10 ? "[VLAN 10 - Офис]" : $"[VLAN {device.VlanId}]";
            string portInfo = port != null ? $"Порт {port.PortNumber}" : "Нет порта";

            var prevColor = GUI.color;
            GUI.color = device.VlanId == 10 ? sameVlanColor : diffVlanColor;

            GUI.Label(new Rect(x + pad, y, w, lineH),
                $"{device.Name} ({device.IP}) {vlanInfo} — {portInfo}", labelStyle);

            GUI.color = prevColor;
            y += lineH;
        }

        y += pad;
        GUI.Label(new Rect(x + pad, y, w, lineH), "--- Тестовые команды ---", titleStyle);
        y += lineH;
        GUI.Label(new Rect(x + pad, y, w, lineH), "NetworkManager → ПКМ в инспекторе:", labelStyle);
        y += lineH;
        GUI.Label(new Rect(x + pad, y, w, lineH), "  Send Test Frame from PC1 to PC2 (VLAN 10 → VLAN 10)", labelStyle);
        y += lineH;
        GUI.Label(new Rect(x + pad, y, w, lineH), "  Send Test Frame from PC1 to PC3 (VLAN 10 → VLAN 20)", labelStyle);
        y += lineH + pad;

        GUI.Label(new Rect(x + pad, y, w, lineH), "[F1] — скрыть/показать  |  [T] — планшет  |  [R] — роутер  |  [L] — ноутбук", labelStyle);
    }
}
