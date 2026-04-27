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

    private void Start()
    {
        saveWanBtn.onClick.AddListener(SaveWAN);
        saveNatBtn.onClick.AddListener(SaveNAT);
        viewTableBtn.onClick.AddListener(ShowTable);

        if (wanSuccessIndicator) wanSuccessIndicator.SetActive(false);
        if (natSuccessIndicator) natSuccessIndicator.SetActive(false);
        tableText.text = "Таблица пуста. Сначала включите NAT.";
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
            tableText.text = "NAT не включен! Таблица недоступна.";
            return;
        }

        tableText.text = "┌───────────────────┬───────────────────┬────────┬─────────┐\n" +
                         "│ Внутри (IP:Порт)  │ Снаружи (IP:Порт) │ Прот.  │ Таймаут │\n" +
                         "├───────────────────┼───────────────────┼────────┼─────────┤\n" +
                         "│ 192.168.1.101:543 │ 203.0.113.10:1234 │ TCP    │ 120с    │\n" +
                         "│ 192.168.1.102:498 │ 203.0.113.10:5400 │ UDP    │ 90с     │\n" +
                         "└───────────────────┴───────────────────┴────────┴─────────┘";

        Level5Manager.Instance.SetStep("Table_Viewed");
    }
}