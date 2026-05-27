using UnityEngine;

public class LevelNetworkSetup : MonoBehaviour
{
    [Header("Создание игрока")]
    public bool createPlayer = true;
    public Vector3 playerSpawn = new Vector3(0, 4, 0);
    public float playerScale = 4f;

    private void Start()
    {
        if (!createPlayer) return;
        if (FindObjectOfType<FirstPersonController>() != null) return;

        var oldCam = Camera.main ?? FindObjectOfType<Camera>();
        if (oldCam != null) Destroy(oldCam.gameObject);

        var go = new GameObject("Player");
        go.transform.position = playerSpawn;
        var cc = go.AddComponent<CharacterController>();
        cc.height = 1.8f * playerScale;
        cc.radius = 0.4f * playerScale;
        cc.center = new Vector3(0, 0.9f * playerScale, 0);

        var cm = new GameObject("PlayerCamera");
        cm.transform.SetParent(go.transform);
        cm.transform.localPosition = new Vector3(0, 0.7f * playerScale, 0);
        cm.tag = "MainCamera";
        var c = cm.AddComponent<Camera>();
        c.nearClipPlane = 0.1f;
        c.farClipPlane = 500f;
        c.fieldOfView = 70;
        cm.AddComponent<AudioListener>();

        go.AddComponent<FirstPersonController>();

        EnsureColliders();
        SetupDevices();
        SetupInteraction();
        SetupTablet();

        var sl = FindObjectOfType<SceneLoader>();
        if (sl != null)
            Level5Manager.Instance.OnLevelCompleted.AddListener(sl.ReturnToCity);

        Debug.Log($"[Setup] Игрок создан (масштаб {playerScale})");
    }

    private void EnsureColliders()
    {
        foreach (var nd in FindObjectsOfType<NetworkDevice>())
        {
            if (nd.GetComponent<Collider>() == null && nd.GetComponent<Renderer>() != null)
                nd.gameObject.AddComponent<MeshCollider>();
        }
        foreach (var objName in new[] { "router", "switcher", "Router", "Switcher" })
        {
            var go = GameObject.Find(objName);
            if (go == null) continue;
            AddColliderRecursive(go);
        }
    }

    private void AddColliderRecursive(GameObject go)
    {
        if (go.GetComponent<Collider>() == null && go.GetComponent<Renderer>() != null)
            go.AddComponent<MeshCollider>();
        foreach (Transform child in go.transform)
            AddColliderRecursive(child.gameObject);
    }

    private void SetupDevices()
    {
        var sw = FindObjectOfType<Switch>();
        if (sw == null)
        {
            Debug.LogWarning("[Setup] Коммутатор не найден!");
            return;
        }

        var allDevices = FindObjectsOfType<NetworkDevice>();
        if (allDevices.Length == 0)
        {
            Debug.LogWarning("[Setup] Сетевые устройства не найдены!");
            return;
        }

        var nm = FindObjectOfType<NetworkManager>();
        if (nm == null)
        {
            nm = gameObject.AddComponent<NetworkManager>();
        }

        var sorted = new System.Collections.Generic.List<NetworkDevice>();
        foreach (var d in allDevices)
        {
            if (d == null || d.GetComponent<Switch>() != null || d.GetComponent<RouterTag>() != null)
                continue;
            sorted.Add(d);
        }
        sorted.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        NetworkDevice pc1 = null, pc2 = null, pc3 = null;
        var ips = new[] { "192.168.1.101", "192.168.1.102", "192.168.1.103" };
        var vlans = new[] { 10, 10, 20 };

        for (int i = 0; i < sorted.Count && i < 3; i++)
        {
            var d = sorted[i];
            d.Name = "PC" + (i + 1);
            d.IP = ips[i];
            d.VlanId = vlans[i];

            var mac = d.GetComponent<MAC>();
            if (mac != null && string.IsNullOrEmpty(d.MacAddress))
                d.MacAddress = mac.MACAddres;
            if (string.IsNullOrEmpty(d.MacAddress))
                d.MacAddress = string.Format("{0:X}:{1:X}:{2:X}:{3:X}:{4:X}:{5:X}",
                    Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256),
                    Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256));

            var ipc = d.GetComponent<IpConfig>();
            if (ipc == null) ipc = d.gameObject.AddComponent<IpConfig>();
            ipc.ip = d.IP;
            ipc.subnetMask = "255.255.255.0";
            ipc.gateway = "192.168.1.1";

            if (i == 0) pc1 = d;
            else if (i == 1) pc2 = d;
            else if (i == 2) pc3 = d;
        }

        var routerGO = GameObject.Find("router") ?? GameObject.Find("Router");
        if (routerGO != null)
        {
            if (routerGO.GetComponent<RouterTag>() == null)
                routerGO.AddComponent<RouterTag>();
            var nd = routerGO.GetComponent<NetworkDevice>();
            if (nd == null) nd = routerGO.AddComponent<NetworkDevice>();
            nd.Name = "Router";
            nd.IP = "203.0.113.10";
            nd.VlanId = 1;
            var rmac = routerGO.GetComponent<MAC>();
            if (rmac == null) rmac = routerGO.AddComponent<MAC>();
            nd.MacAddress = rmac.MACAddres;
            var ripc = routerGO.GetComponent<IpConfig>();
            if (ripc == null) ripc = routerGO.AddComponent<IpConfig>();
            ripc.ip = nd.IP;
            ripc.subnetMask = "255.255.255.252";
            ripc.gateway = "203.0.113.9";
        }

        var switchGO = GameObject.Find("switcher") ?? GameObject.Find("Switch") ?? sw?.gameObject;
        if (switchGO != null)
        {
            var snd = switchGO.GetComponent<NetworkDevice>();
            if (snd == null) snd = switchGO.AddComponent<NetworkDevice>();
            snd.Name = "Switch";
            snd.IP = "192.168.1.254";
            snd.VlanId = 1;
            if (switchGO.GetComponent<SimpleHighlight>() == null)
                switchGO.AddComponent<SimpleHighlight>();
        }

        nm.switchDevice = sw;
        nm.pc1 = pc1;
        nm.pc2 = pc2;
        nm.pc3 = pc3;

        var dc = FindObjectOfType<DemoController>();
        if (dc == null)
        {
            dc = gameObject.AddComponent<DemoController>();
        }
        dc.networkManager = nm;

        var vdui = FindObjectOfType<VlanDebugUI>();
        if (vdui == null)
        {
            vdui = gameObject.AddComponent<VlanDebugUI>();
        }
        vdui.networkManager = nm;
    }

    private void SetupInteraction()
    {
        var di = GetComponent<DeviceInteraction>();
        if (di == null) di = gameObject.AddComponent<DeviceInteraction>();
    }

    private void SetupTablet()
    {
        var t = FindObjectOfType<TabletInfoPanel>();
        if (t == null)
        {
            t = gameObject.AddComponent<TabletInfoPanel>();
        }
    }
}
