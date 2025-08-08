using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LineCleaner : MonoBehaviour
{
    private Button cleanButton;

    private void Awake()
    {
        // Получаем компонент кнопки
        cleanButton = GetComponent<Button>();

        // Подписываемся на событие нажатия
        cleanButton.onClick.AddListener(CleanAllLines);
    }

    public void CleanAllLines()
    {
        // Находим все линии в сцене
        LineRenderer[] allLines = FindObjectsOfType<LineRenderer>();

        // Уничтожаем каждый объект с LineRenderer
        foreach (LineRenderer line in allLines)
        {
            if (line.gameObject.name == "ConnectionLine")
            {
                Destroy(line.gameObject);
            }
        }

        Debug.Log("Все линии удалены!");
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (cleanButton != null)
        {
            cleanButton.onClick.RemoveListener(CleanAllLines);
        }
    }
}