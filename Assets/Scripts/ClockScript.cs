using UnityEngine;
using System;
using TMPro;

public class ClockScript : MonoBehaviour
{
    public TextMeshProUGUI timeText; // UI элемент для отображения времени

    private void Awake()
    {
        timeText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        DateTime now = DateTime.Now;
        timeText.text = now.ToString("HH:mm\ndd/MM/yyyy"); // Форматирует время в часы:минуты
    }
}

