using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    public Text[] stepLabels; // 6 элементов
    public Image[] stepIcons; // 6 элементов
    public Sprite checkSprite;
    public Sprite crossSprite;

    public void ShowStats()
    {
        gameObject.SetActive(true);

        bool[] flags = {
            Level5Manager.Instance.isDiagnosed,
            Level5Manager.Instance.isWanConfigured,
            Level5Manager.Instance.isNatEnabled,
            Level5Manager.Instance.isInternetOk,
            Level5Manager.Instance.isTableSeen,
            Level5Manager.Instance.isQuizPassed
        };

        string[] names = { "Диагностика", "WAN настроен", "NAT включён", "Пинг OK", "Таблица просмотрена", "Тест сдан" };

        for (int i = 0; i < 6; i++)
        {
            if (i < stepLabels.Length) stepLabels[i].text = names[i];
            if (i < stepIcons.Length) stepIcons[i].sprite = flags[i] ? checkSprite : crossSprite;
        }
    }
}