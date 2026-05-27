using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LaptopUI : MonoBehaviour
{
    [Header("Интерфейс")]
    public TMPro.TMP_Text terminalText;
    public GameObject browserPanel;

    [Header("Кнопки")]
    public Button btnPingExt;
    public Button btnPingRouter;
    public Button btnPingPrinter;
    public Button btnOpenBrowser;

    private bool isDiagnosedTriggered = false;
    private GameObject mainControls;
    private GameObject quizUI;
    private Toggle[][] quizToggles;
    private Text quizResultText;
    private Button submitQuizBtn;
    private Button quizCloseBtn;

    private struct QuizData { public string question; public string[] options; public int correctIndex; }
    private QuizData[] quizQuestions;

    private Font uiFont;

    private void Start()
    {
        Debug.Log("[LaptopUI] Инициализация...");

        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (uiFont == null) uiFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (uiFont == null) uiFont = Font.CreateDynamicFontFromOSFont("Arial", 20);
        if (uiFont == null) Debug.LogError("[LaptopUI] Шрифт НЕ ЗАГРУЖЕН!");
        else Debug.Log("[LaptopUI] Шрифт: " + uiFont.name);

        CreateMainControlsWrapper();
        InitQuizData();
        LogRefs();
        BindButtons();

        if (browserPanel != null)
        {
            browserPanel.SetActive(false);
            var bg = browserPanel.GetComponent<Image>();
            if (bg != null) bg.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        }

        if (terminalText != null) terminalText.text = "C:\\User\\Guest> _";
    }

    private void CreateMainControlsWrapper()
    {
        mainControls = new GameObject("MainControls");
        mainControls.transform.SetParent(transform, false);
        var mcRect = mainControls.AddComponent<RectTransform>();
        mcRect.anchorMin = Vector2.zero;
        mcRect.anchorMax = Vector2.one;
        mcRect.sizeDelta = Vector2.zero;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            GameObject go = child.gameObject;
            if (go != mainControls && go != browserPanel)
                child.SetParent(mainControls.transform, true);
        }
    }

    private void InitQuizData()
    {
        quizQuestions = new QuizData[3];
        quizQuestions[0] = new QuizData
        {
            question = "1. Какой стандарт определяет VLAN?",
            options = new[] { "IEEE 802.1Q", "IEEE 802.3", "IEEE 802.11" },
            correctIndex = 0
        };
        quizQuestions[1] = new QuizData
        {
            question = "2. Сколько VLAN настроено в работе?",
            options = new[] { "1", "2", "3" },
            correctIndex = 1
        };
        quizQuestions[2] = new QuizData
        {
            question = "3. Какое устройство маршрутизирует между VLAN?",
            options = new[] { "Концентратор", "Коммутатор", "Маршрутизатор" },
            correctIndex = 2
        };
    }

    private void LogRefs()
    {
        if (terminalText == null) Debug.LogError("[LaptopUI] terminalText = NONE");
        if (browserPanel == null) Debug.LogError("[LaptopUI] browserPanel = NONE");
        if (btnPingExt == null) Debug.LogError("[LaptopUI] btnPingExt = NONE");
        if (btnPingRouter == null) Debug.LogError("[LaptopUI] btnPingRouter = NONE");
        if (btnPingPrinter == null) Debug.LogError("[LaptopUI] btnPingPrinter = NONE");
        if (btnOpenBrowser == null) Debug.LogError("[LaptopUI] btnOpenBrowser = NONE");
    }

    private void BindButtons()
    {
        if (btnPingExt != null) btnPingExt.onClick.AddListener(() => RunPing("8.8.8.8"));
        if (btnPingRouter != null) btnPingRouter.onClick.AddListener(() => RunPing("192.168.1.1"));
        if (btnPingPrinter != null) btnPingPrinter.onClick.AddListener(() => RunPing("192.168.1.50"));
        if (btnOpenBrowser != null) btnOpenBrowser.onClick.AddListener(ToggleBrowser);
    }

    private void RunPing(string target)
    {
        Debug.Log($"[LaptopUI] ping {target}");
        string result;
        if (target == "8.8.8.8")
        {
            bool netReady = Level5Manager.Instance.isWanConfigured && Level5Manager.Instance.isNatEnabled;
            if (netReady)
            {
                result = "Ответ от 8.8.8.8: время=14мс TTL=56\nСтатистика: пакетов: 1, получено: 1, потеряно: 0 (0%)";
                Level5Manager.Instance.SetStep("Ping_OK");
            }
            else result = "Превышен интервал ожидания для запроса.";
        }
        else
        {
            result = $"Ответ от {target}: время<1мс TTL=128";
            if (!isDiagnosedTriggered) { Level5Manager.Instance.SetStep("Diagnosed"); isDiagnosedTriggered = true; }
        }
        if (terminalText != null) terminalText.text += $"\n\nping {target}\n{result}\nC:\\User\\Guest> _";
    }

    private void ToggleBrowser()
    {
        bool internet = Level5Manager.Instance != null && Level5Manager.Instance.isInternetOk;
        if (!internet)
        {
            if (terminalText != null)
                terminalText.text += "\n\n[ОШИБКА] Нет интернета. Настройте WAN и NAT.";
            return;
        }

        bool isOpen = browserPanel != null && browserPanel.activeSelf;
        if (!isOpen) OpenBrowser();
        else CloseBrowser();
    }

    private void OpenBrowser()
    {
        Debug.Log("[LaptopUI] OpenBrowser()");
        if (browserPanel == null) { Debug.LogError("[LaptopUI] browserPanel=null"); return; }
        if (mainControls != null) mainControls.SetActive(false);
        browserPanel.SetActive(true);
        Debug.Log("[LaptopUI] quizUI==null? " + (quizUI == null));
        if (quizUI == null) CreateQuizUI();
    }

    private void CloseBrowser()
    {
        if (browserPanel != null) browserPanel.SetActive(false);
        if (mainControls != null) mainControls.SetActive(true);
    }

    private void CreateQuizUI()
    {
        Debug.Log("[LaptopUI] CreateQuizUI() старт");
        Debug.Log("[LaptopUI] Детей browserPanel до очистки: " + browserPanel.transform.childCount);
        foreach (Transform t in browserPanel.transform)
            Destroy(t.gameObject);
        Debug.Log("[LaptopUI] Детей после Destroy: " + browserPanel.transform.childCount);

        quizUI = new GameObject("QuizContent");
        quizUI.transform.SetParent(browserPanel.transform, false);
        var quizRect = quizUI.AddComponent<RectTransform>();
        quizRect.anchorMin = Vector2.zero;
        quizRect.anchorMax = Vector2.one;
        quizRect.sizeDelta = Vector2.zero;

        var title = MakeText(quizRect, "Title", "<b>Квиз по VLAN</b>", 30, TextAnchor.UpperCenter);
        title.rectTransform.anchorMin = new Vector2(0, 0.85f);
        title.rectTransform.anchorMax = new Vector2(1, 1);
        title.rectTransform.offsetMin = Vector2.zero;
        title.rectTransform.offsetMax = Vector2.zero;

        quizToggles = new Toggle[3][];
        float[] yStarts = { 0.62f, 0.37f, 0.12f };

        for (int q = 0; q < 3; q++)
        {
            var qRect = MakeText(quizRect, $"Q{q + 1}", quizQuestions[q].question, 22, TextAnchor.UpperLeft);
            qRect.rectTransform.anchorMin = new Vector2(0.03f, yStarts[q] + 0.12f);
            qRect.rectTransform.anchorMax = new Vector2(0.97f, yStarts[q] + 0.18f);
            qRect.rectTransform.offsetMin = Vector2.zero;
            qRect.rectTransform.offsetMax = Vector2.zero;

            var tgGO = new GameObject($"ToggleGroup_Q{q + 1}", typeof(RectTransform));
            tgGO.transform.SetParent(quizRect, false);
            var tg = tgGO.AddComponent<ToggleGroup>();
            tg.allowSwitchOff = true;
            quizToggles[q] = new Toggle[3];
            for (int o = 0; o < 3; o++)
            {
                float xOff = 0.01f + o * 0.33f;
                quizToggles[q][o] = MakeToggle(tgGO.transform, $"Q{q + 1}_A{o + 1}",
                    quizQuestions[q].options[o], xOff, 0.1f, 0.31f, 0.8f, tg);
            }
            var tgRect = tgGO.GetComponent<RectTransform>();
            tgRect.anchorMin = new Vector2(0.02f, yStarts[q]);
            tgRect.anchorMax = new Vector2(0.98f, yStarts[q] + 0.1f);
            tgRect.offsetMin = Vector2.zero;
            tgRect.offsetMax = Vector2.zero;
        }

        submitQuizBtn = MakeButton(quizRect, "SubmitBtn", "Проверить", 0.38f, 0.01f, 0.24f, 0.06f, new Color(0.2f, 0.6f, 0.2f));
        submitQuizBtn.onClick.AddListener(SubmitQuiz);

        quizResultText = MakeText(quizRect, "Result", "", 22, TextAnchor.MiddleCenter);
        quizResultText.rectTransform.anchorMin = new Vector2(0.2f, 0.08f);
        quizResultText.rectTransform.anchorMax = new Vector2(0.8f, 0.14f);
        quizResultText.rectTransform.offsetMin = Vector2.zero;
        quizResultText.rectTransform.offsetMax = Vector2.zero;
        quizResultText.raycastTarget = false;

        quizCloseBtn = MakeButton(quizRect, "CloseBrowserBtn", "Закрыть", 0.82f, 0.9f, 0.15f, 0.06f, new Color(0.6f, 0.2f, 0.2f));
        quizCloseBtn.onClick.AddListener(CloseBrowser);
        Debug.Log("[LaptopUI] CreateQuizUI() конец - quizToggles[0].Length=" + (quizToggles[0]?.Length ?? -1));
    }

    private Text MakeText(Transform parent, string name, string content, int size, TextAnchor align)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var text = go.AddComponent<Text>();
        text.font = uiFont;
        text.fontSize = size;
        text.color = Color.white;
        text.alignment = align;
        text.supportRichText = true;
        text.text = content;
        return text;
    }

    private Toggle MakeToggle(Transform parent, string name, string label,
        float x, float y, float w, float h, ToggleGroup group)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(x, y);
        rect.anchorMax = new Vector2(x + w, y + h);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var toggle = go.AddComponent<Toggle>();
        toggle.group = group;

        var bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        var bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.25f, 0.25f, 0.25f);

        var check = new GameObject("Checkmark");
        check.transform.SetParent(go.transform, false);
        var ckRect = check.AddComponent<RectTransform>();
        ckRect.anchorMin = new Vector2(0.02f, 0.15f);
        ckRect.anchorMax = new Vector2(0.12f, 0.85f);
        ckRect.sizeDelta = Vector2.zero;
        var ckImg = check.AddComponent<Image>();
        ckImg.color = new Color(0.3f, 0.8f, 0.3f);
        toggle.graphic = ckImg;
        toggle.targetGraphic = bgImg;
        check.SetActive(false);

        toggle.onValueChanged.AddListener((on) => check.SetActive(on));

        var lbl = new GameObject("Label");
        lbl.transform.SetParent(go.transform, false);
        var lRect = lbl.AddComponent<RectTransform>();
        lRect.anchorMin = new Vector2(0.15f, 0);
        lRect.anchorMax = new Vector2(1, 1);
        lRect.sizeDelta = Vector2.zero;
        var lText = lbl.AddComponent<Text>();
        lText.font = uiFont;
        lText.fontSize = 18;
        lText.color = Color.white;
        lText.alignment = TextAnchor.MiddleLeft;
        lText.text = label;

        return toggle;
    }

    private Button MakeButton(Transform parent, string name, string label,
        float x, float y, float w, float h, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(x, y);
        rect.anchorMax = new Vector2(x + w, y + h);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var img = go.AddComponent<Image>();
        img.color = color;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var lbl = new GameObject("Label");
        lbl.transform.SetParent(go.transform, false);
        var lRect = lbl.AddComponent<RectTransform>();
        lRect.anchorMin = Vector2.zero;
        lRect.anchorMax = Vector2.one;
        lRect.sizeDelta = Vector2.zero;
        var lText = lbl.AddComponent<Text>();
        lText.font = uiFont;
        lText.fontSize = 20;
        lText.color = Color.white;
        lText.alignment = TextAnchor.MiddleCenter;
        lText.text = label;

        return btn;
    }

    private void SubmitQuiz()
    {
        bool allCorrect = true;
        for (int q = 0; q < 3 && allCorrect; q++)
        {
            bool answered = false;
            bool correct = false;
            for (int o = 0; o < 3; o++)
            {
                if (quizToggles[q][o].isOn)
                {
                    answered = true;
                    if (o == quizQuestions[q].correctIndex) correct = true;
                    break;
                }
            }
            if (!answered || !correct) allCorrect = false;
        }

        if (allCorrect)
        {
            quizResultText.text = "Все ответы верны! Уровень пройден!";
            quizResultText.color = Color.green;
            Level5Manager.Instance.SetStep("Quiz_Passed");
        }
        else
        {
            quizResultText.text = "Есть ошибки. Попробуйте снова.";
            quizResultText.color = Color.red;
        }
    }
}
