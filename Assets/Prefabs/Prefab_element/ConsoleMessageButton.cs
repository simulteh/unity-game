using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConsoleMessageButton : MonoBehaviour
{
    [Header("Настройки сообщения")]
    [Tooltip("Текст сообщения для вывода в консоль")]
    public string message = "Кнопка была нажата!";


    private Button button;

    private void Awake()
    {
        // Получаем компонент кнопки
        button = GetComponent<Button>();

        // Подписываемся на событие нажатия
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // Формируем сообщение
        string coloredMessage = $"<color=#>{message}</color>";

        // Выводим в консоль
        Debug.Log(message);
    }


}