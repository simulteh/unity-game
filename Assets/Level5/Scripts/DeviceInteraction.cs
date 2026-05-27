using UnityEngine;
using UnityEngine.UI;

public class DeviceInteraction : MonoBehaviour
{
    [Header("UI панели")]
    public GameObject laptopPanel;
    public GameObject routerPanel;

    private GameObject selectedDevice;
    private GameObject prevDevice;
    private GameObject deviceInfoPanel;
    private Text infoNameText;
    private Text infoDetailsText;
    private Camera cam;
    private FirstPersonController fpc;

    private void Start()
    {
        if (laptopPanel == null || routerPanel == null)
        {
            var canvas = GameObject.Find("Canvas");
            if (canvas != null)
            {
                foreach (Transform child in canvas.transform)
                {
                    if (laptopPanel == null && child.name == "LaptopUI_Panel")
                        laptopPanel = child.gameObject;
                    if (routerPanel == null && child.name == "RouterUI_Panel")
                        routerPanel = child.gameObject;
                }
            }
        }
        CreateDeviceInfoPanel();
    }

    private void CreateDeviceInfoPanel()
    {
        var parent = GameObject.Find("Canvas");
        if (parent == null) return;

        var panelGO = new GameObject("DeviceInfoPanel");
        panelGO.transform.SetParent(parent.transform, false);
        var rect = panelGO.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = new Vector2(360, 160);
        var img = panelGO.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);

        var nameGO = new GameObject("InfoName");
        nameGO.transform.SetParent(panelGO.transform, false);
        var nameRect = nameGO.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 1);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.pivot = new Vector2(0, 1);
        nameRect.anchoredPosition = new Vector2(15, -15);
        nameRect.sizeDelta = new Vector2(-30, 40);
        infoNameText = nameGO.AddComponent<Text>();
        infoNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        infoNameText.fontSize = 20;
        infoNameText.fontStyle = FontStyle.Bold;
        infoNameText.color = Color.white;
        infoNameText.alignment = TextAnchor.UpperLeft;

        var detailsGO = new GameObject("InfoDetails");
        detailsGO.transform.SetParent(panelGO.transform, false);
        var detailsRect = detailsGO.AddComponent<RectTransform>();
        detailsRect.anchorMin = new Vector2(0, 0);
        detailsRect.anchorMax = new Vector2(1, 1);
        detailsRect.pivot = new Vector2(0, 1);
        detailsRect.anchoredPosition = new Vector2(15, -55);
        detailsRect.sizeDelta = new Vector2(-30, -70);
        infoDetailsText = detailsGO.AddComponent<Text>();
        infoDetailsText.font = infoNameText.font;
        infoDetailsText.fontSize = 16;
        infoDetailsText.color = Color.white;
        infoDetailsText.alignment = TextAnchor.UpperLeft;

        deviceInfoPanel = panelGO;
        deviceInfoPanel.SetActive(false);
    }

    private void Update()
    {
        if (cam == null) cam = Camera.main ?? FindObjectOfType<Camera>();
        if (cam == null) return;
        if (fpc == null) fpc = FindObjectOfType<FirstPersonController>();

        bool anyOpen = (laptopPanel != null && laptopPanel.activeSelf) ||
                       (routerPanel != null && routerPanel.activeSelf);
        if (!anyOpen && fpc != null && !fpc.ControlsEnabled)
            fpc.ControlsEnabled = true;

        if (Input.GetMouseButtonDown(0))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            var go = hit.collider.gameObject;
            var device = go.GetComponentInParent<NetworkDevice>();
            var sw = go.GetComponentInParent<Switch>();
            var rt = go.GetComponentInParent<RouterTag>();

            if (routerPanel != null && routerPanel.activeSelf && rt != null) return;

            if (device != null && device.Name != null && device.Name.StartsWith("PC"))
            {
                HideDeviceInfo();
                SelectDevice(device.gameObject);
                TogglePanel(laptopPanel);
                return;
            }

            if (rt != null)
            {
                HideDeviceInfo();
                TogglePanel(routerPanel);
                return;
            }

            if (device != null)
            {
                SelectDevice(device.gameObject);
                ShowDeviceInfo(device);
                return;
            }

            if (sw != null)
            {
                HideDeviceInfo();
                SelectDevice(sw.gameObject);
                var nd = sw.GetComponent<NetworkDevice>();
                if (nd != null) ShowDeviceInfo(nd);
                return;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            CloseAllPanels();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllPanels();
        }
    }

    private void CloseAllPanels()
    {
        if (laptopPanel != null && laptopPanel.activeSelf)
        {
            laptopPanel.SetActive(false);
            if (fpc != null) fpc.ControlsEnabled = true;
        }
        if (routerPanel != null && routerPanel.activeSelf)
        {
            routerPanel.SetActive(false);
            if (fpc != null) fpc.ControlsEnabled = true;
        }
        HideDeviceInfo();
    }

    private void ShowDeviceInfo(NetworkDevice device)
    {
        if (deviceInfoPanel == null) return;
        infoNameText.text = device.Name;
        infoDetailsText.text = $"IP: {device.IP}\nMAC: {device.MacAddress}\nVLAN: {device.VlanId}";
        deviceInfoPanel.SetActive(true);
    }

    private void HideDeviceInfo()
    {
        if (deviceInfoPanel != null && deviceInfoPanel.activeSelf)
        {
            deviceInfoPanel.SetActive(false);
            if (fpc != null) fpc.ControlsEnabled = true;
        }
        ClearSelection();
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;
        bool active = !panel.activeSelf;
        panel.SetActive(active);
        if (fpc != null)
            fpc.ControlsEnabled = !active;
    }

    private void SelectDevice(GameObject go)
    {
        ClearSelection();
        selectedDevice = go;
        var sh = go.GetComponent<SimpleHighlight>();
        if (sh != null) sh.Highlight();
    }

    private void ClearSelection()
    {
        if (prevDevice != null)
        {
            var ph = prevDevice.GetComponent<SimpleHighlight>();
            if (ph != null) ph.Unhighlight();
        }
        prevDevice = selectedDevice;
        if (selectedDevice != null)
        {
            var sh = selectedDevice.GetComponent<SimpleHighlight>();
            if (sh != null) sh.Unhighlight();
        }
        selectedDevice = null;
    }
}

public class RouterTag : MonoBehaviour { }
