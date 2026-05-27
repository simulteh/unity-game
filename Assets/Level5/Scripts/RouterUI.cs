using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RouterUI : MonoBehaviour
{
    [Header("Поля WAN")]
    public TMPro.TMP_InputField wanIP;
    public TMPro.TMP_InputField wanMask;
    public TMPro.TMP_InputField wanGW;
    public GameObject wanSuccessIndicator;

    [Header("NAT")]
    public Toggle natToggle;
    public GameObject natSuccessIndicator;

    [Header("Кнопки")]
    public Button saveWanBtn;
    public Button saveNatBtn;
    public Button viewTableBtn;

    [Header("Таблица трансляций")]
    public TMPro.TMP_Text tableText;

    private Button closeBtn;
    private GameObject natTablePanel;

    private void Start()
    {
        saveWanBtn.onClick.AddListener(SaveWAN);
        saveNatBtn.onClick.AddListener(SaveNAT);
        viewTableBtn.onClick.AddListener(ShowTable);

        if (wanSuccessIndicator) wanSuccessIndicator.SetActive(false);
        if (natSuccessIndicator) natSuccessIndicator.SetActive(false);
        tableText.text = "Таблица пуста. Сначала включите NAT.";

        CreateCloseButton();
    }

    private void CreateCloseButton()
    {
        var btnGO = new GameObject("CloseRouterBtn", typeof(RectTransform));
        btnGO.transform.SetParent(transform, false);
        closeBtn = btnGO.AddComponent<Button>();
        var rect = btnGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.85f, 0.85f);
        rect.anchorMax = new Vector2(0.98f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        var img = btnGO.AddComponent<Image>();
        img.color = new Color(0.7f, 0.15f, 0.15f);
        img.raycastTarget = true;
        var lbl = new GameObject("Label", typeof(RectTransform));
        lbl.transform.SetParent(btnGO.transform, false);
        var lRect = lbl.GetComponent<RectTransform>();
        lRect.anchorMin = Vector2.zero;
        lRect.anchorMax = Vector2.one;
        lRect.offsetMin = Vector2.zero;
        lRect.offsetMax = Vector2.zero;
        var lText = lbl.AddComponent<Text>();
        lText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (lText.font == null) lText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        lText.fontSize = 22;
        lText.color = Color.white;
        lText.alignment = TextAnchor.MiddleCenter;
        lText.text = "Закрыть";
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void SaveWAN()
    {
        if (wanIP == null || wanMask == null || wanGW == null)
        {
            Debug.LogError("[RouterUI] Не привязаны поля WAN!");
            return;
        }

        string ip = wanIP.text.Trim();
        string mask = wanMask.text.Trim();
        string gw = wanGW.text.Trim();

        Debug.Log($"[DEBUG WAN] IP='{ip}' Mask='{mask}' GW='{gw}'");
        Debug.Log($"[DEBUG WAN] Ожидается: IP='203.0.113.10' Mask='255.255.255.252' GW='203.0.113.9'");

        bool isCorrect = ip == "203.0.113.10" && mask == "255.255.255.252" && gw == "203.0.113.9";

        if (wanSuccessIndicator) wanSuccessIndicator.SetActive(isCorrect);
        if (isCorrect)
        {
            Level5Manager.Instance.SetStep("WAN_Done");
            Debug.Log("[Router] WAN настроен успешно!");
        }
        else
            Debug.Log("[Router] Ошибка: неверные данные провайдера!");
    }

    private void SaveNAT()
    {
        if (natToggle == null) { Debug.LogError("[NAT] ⛔ Поле Nat Toggle равно None!"); return; }
        if (Level5Manager.Instance == null) { Debug.LogError("[NAT] ⛔ LevelManager не найден в сцене!"); return; }

        bool state = natToggle.isOn;
        Debug.Log($"[DEBUG NAT] Клик! Тумблер сейчас: {state}");

        if (state)
        {
            Level5Manager.Instance.SetStep("NAT_On");
            Debug.Log("[NAT] ✅ Метод SetStep('NAT_On') вызван! Флаг должен стать TRUE.");
            if (natSuccessIndicator != null) natSuccessIndicator.SetActive(true);
        }
        else
        {
            Debug.Log("[NAT] ⚠️ Тумблер ВЫКЛЮЧЕН. Поставь галочку и нажми снова.");
            if (natSuccessIndicator != null) natSuccessIndicator.SetActive(false);
        }
    }

    private void ShowTable()
    {
        if (!Level5Manager.Instance.isNatEnabled)
        {
            ShowNatTablePopup("NAT не включен!\nСначала включите NAT.");
            return;
        }

        string table = "┌───────────────────┬───────────────────┬────────┬─────────┐\n" +
                       "│  Внутри (IP:Порт)  │ Снаружи (IP:Порт) │ Прот.  │ Таймаут │\n" +
                       "├───────────────────┼───────────────────┼────────┼─────────┤\n" +
                       "│ 192.168.1.101:543 │ 203.0.113.10:1234 │ TCP    │ 120с    │\n" +
                       "│ 192.168.1.102:498 │ 203.0.113.10:5400 │ UDP    │ 90с     │\n" +
                       "└───────────────────┴───────────────────┴────────┴─────────┘";

        ShowNatTablePopup(table);
        Level5Manager.Instance.SetStep("Table_Viewed");
    }

    private void ShowNatTablePopup(string content)
    {
        if (natTablePanel != null) { Destroy(natTablePanel); natTablePanel = null; }

        var parent = GameObject.Find("Canvas");
        if (parent == null) return;

        var panelGO = new GameObject("NatTablePopup", typeof(RectTransform));
        panelGO.transform.SetParent(parent.transform, false);
        var rect = panelGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, 0.2f);
        rect.anchorMax = new Vector2(0.9f, 0.8f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var img = panelGO.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.85f);

        var textGO = new GameObject("TableContent", typeof(RectTransform));
        textGO.transform.SetParent(panelGO.transform, false);
        var tRect = textGO.GetComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero;
        tRect.anchorMax = Vector2.one;
        tRect.offsetMin = new Vector2(30, 50);
        tRect.offsetMax = new Vector2(-30, -30);

        var tText = textGO.AddComponent<Text>();
        tText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (tText.font == null) tText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        tText.fontSize = 28;
        tText.color = Color.white;
        tText.alignment = TextAnchor.MiddleLeft;
        tText.supportRichText = true;
        tText.text = content;

        var closePopup = new GameObject("ClosePopupBtn", typeof(RectTransform));
        closePopup.transform.SetParent(panelGO.transform, false);
        var cRect = closePopup.GetComponent<RectTransform>();
        cRect.anchorMin = new Vector2(0.85f, 0.85f);
        cRect.anchorMax = new Vector2(0.95f, 0.95f);
        cRect.offsetMin = Vector2.zero;
        cRect.offsetMax = Vector2.zero;
        var cImg = closePopup.AddComponent<Image>();
        cImg.color = new Color(0.6f, 0.2f, 0.2f);
        var cBtn = closePopup.AddComponent<Button>();
        var cLbl = new GameObject("CLabel", typeof(RectTransform));
        cLbl.transform.SetParent(closePopup.transform, false);
        var clRect = cLbl.GetComponent<RectTransform>();
        clRect.anchorMin = Vector2.zero;
        clRect.anchorMax = Vector2.one;
        clRect.offsetMin = Vector2.zero;
        clRect.offsetMax = Vector2.zero;
        var clText = cLbl.AddComponent<Text>();
        clText.font = tText.font;
        clText.fontSize = 18;
        clText.color = Color.white;
        clText.alignment = TextAnchor.MiddleCenter;
        clText.text = "X";
        cBtn.onClick.AddListener(() => Destroy(panelGO));

        natTablePanel = panelGO;
    }
}
