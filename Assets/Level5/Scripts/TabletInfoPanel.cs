using UnityEngine;
using UnityEngine.UI;

public class TabletInfoPanel : MonoBehaviour
{
    [Header("Display Settings")]
    public KeyCode toggleKey = KeyCode.T;
    public GameObject tabletCanvas;

    private Text progressText;

    private void Start()
    {
        if (tabletCanvas == null)
            CreateTabletCanvas();
        tabletCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && tabletCanvas != null)
        {
            bool show = !tabletCanvas.activeSelf;
            tabletCanvas.SetActive(show);
            if (show) RefreshProgress();
        }
    }

    private void RefreshProgress()
    {
        if (progressText == null) return;
        var m = Level5Manager.Instance;
        if (m == null) return;

        bool[] flags = {
            m.isDiagnosed, m.isWanConfigured, m.isNatEnabled,
            m.isInternetOk, m.isTableSeen, m.isQuizPassed
        };
        string[] names = {
            "Диагностика сети (пинг)", "WAN настроен", "NAT включён",
            "Интернет работает", "Таблица NAT", "Квиз пройден"
        };

        string result = "<b><size=24>ПРОГРЕСС ВЫПОЛНЕНИЯ</size></b>\n\n";
        int done = 0;
        for (int i = 0; i < 6; i++)
        {
            result += (flags[i] ? "✅ " : "⬜ ") + names[i] + "\n";
            if (flags[i]) done++;
        }
        result += $"\n<b>Выполнено: {done}/6</b>";
        if (done >= 6) result += "\n\n<b><color=green>УРОВЕНЬ ПРОЙДЕН!</color></b>";

        progressText.text = result;
    }

    private void CreateTabletCanvas()
    {
        var canvasGO = new GameObject("TabletInfoCanvas");
        canvasGO.layer = 5;

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(canvasGO.transform, false);
        var bgRect = bgGO.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0, 0, 0, 0.7f);

        var panelGO = new GameObject("InfoPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        var panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.1f, 0.1f);
        panelRect.anchorMax = new Vector2(0.9f, 0.9f);
        panelRect.sizeDelta = Vector2.zero;
        var panelImg = panelGO.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null) font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Arial", 20);
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Times New Roman", 20);
        if (font == null) font = Font.CreateDynamicFontFromOSFont("Segoe UI", 20);
        if (font == null) Debug.LogWarning("[Tablet] Шрифт не найден");
        else Debug.Log($"[Tablet] Шрифт загружен: {font.name}");

        var descGO = new GameObject("DescriptionText");
        descGO.transform.SetParent(panelGO.transform, false);
        var descRect = descGO.AddComponent<RectTransform>();
        descRect.anchorMin = Vector2.zero;
        descRect.anchorMax = new Vector2(0.5f, 1);
        descRect.offsetMin = new Vector2(20, 20);
        descRect.offsetMax = new Vector2(-10, -20);
        var descText = descGO.AddComponent<Text>();
        descText.font = font;
        descText.fontSize = 20;
        descText.color = Color.white;
        descText.alignment = TextAnchor.UpperLeft;
        descText.supportRichText = true;
        descText.text = GetDefaultDescription();

        var progGO = new GameObject("ProgressText");
        progGO.transform.SetParent(panelGO.transform, false);
        var progRect = progGO.AddComponent<RectTransform>();
        progRect.anchorMin = new Vector2(0.5f, 0);
        progRect.anchorMax = new Vector2(1, 1);
        progRect.offsetMin = new Vector2(10, 20);
        progRect.offsetMax = new Vector2(-20, -20);
        progressText = progGO.AddComponent<Text>();
        progressText.font = font;
        progressText.fontSize = 20;
        progressText.color = Color.white;
        progressText.alignment = TextAnchor.UpperLeft;
        progressText.supportRichText = true;
        progressText.text = "";

        tabletCanvas = canvasGO;
        Debug.Log("[Tablet] Canvas создан, нажми [T] для показа");
    }

    private string GetDefaultDescription()
    {
        return
"<b><size=24>ПРОЕКТ: Имитация работы\nкоммутатора с VLAN</size></b>\n\n" +
"<b>Разработали:</b> Киор Петр, Крумин Александр, Шитик Алина, Ценев Алексей, Козлов Никита\n" +
"<b>Дисциплина:</b> Производственная практика\n\n" +
"━━━━━━━━━━━━━━━━━\n\n" +
"<b>УПРАВЛЕНИЕ:</b>\n" +
"• ЛКМ по ПК — ноутбук\n" +
"• ЛКМ по роутеру — настройка\n" +
"• ЛКМ по коммутатору — инфа\n" +
"• [F1] — VLAN Debug\n" +
"• [T] — планшет\n\n" +
"━━━━━━━━━━━━━━━━━\n\n" +
"<b>СЦЕНАРИЙ:</b>\n" +
"1. Пингануть ПК\n" +
"2. Настроить WAN на роутере\n" +
"3. Включить NAT\n" +
"4. Пингануть 8.8.8.8\n" +
"5. Открыть браузер\n" +
"6. Пройти квиз\n\n" +
"<i>Нажми [T] для закрытия</i>";
    }
}
