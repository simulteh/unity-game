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

    private void Start()
    {
        Debug.Log("[LaptopUI] 🔍 Проверка привязок...");

        if (terminalText == null) Debug.LogError("[LaptopUI] ⛔ Поле Terminal Text = None!");
        if (browserPanel == null) Debug.LogError("[LaptopUI] ⛔ Поле Browser Panel = None!");
        if (btnPingExt == null) Debug.LogError("[LaptopUI] ⛔ Поле Btn Ping Ext = None!");
        if (btnPingRouter == null) Debug.LogError("[LaptopUI] ⛔ Поле Btn Ping Router = None!");
        if (btnPingPrinter == null) Debug.LogError("[LaptopUI] ⛔ Поле Btn Ping Printer = None!");
        if (btnOpenBrowser == null) Debug.LogError("[LaptopUI] ⛔ Поле Btn Open Browser = None!");

        // Привязка (сработает только если поле НЕ равно null)
        if (btnPingExt != null) btnPingExt.onClick.AddListener(() => RunPing("8.8.8.8"));
        if (btnPingRouter != null) btnPingRouter.onClick.AddListener(() => RunPing("192.168.1.1"));
        if (btnPingPrinter != null) btnPingPrinter.onClick.AddListener(() => RunPing("192.168.1.50"));
        if (btnOpenBrowser != null) btnOpenBrowser.onClick.AddListener(OpenBrowser);

        if (browserPanel != null) browserPanel.SetActive(false);
        if (terminalText != null) terminalText.text = "C:\\User\\Guest> _";

        Debug.Log("[LaptopUI] ✅ Инициализация завершена. Жду кликов.");
    }

    private void RunPing(string target)
    {
        Debug.Log($"[LaptopUI] 🖱️ Клик: ping {target}");
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

    private void OpenBrowser()
    {
        Debug.Log("[LaptopUI] 🖱️ Клик: Открыть браузер");
        if (Level5Manager.Instance.isInternetOk)
        {
            if (browserPanel != null) browserPanel.SetActive(true);
        }
        else
        {
            if (terminalText != null) terminalText.text += "\n\n[ОШИБКА] Нет интернета. Настройте WAN и NAT.";
        }
    }
}