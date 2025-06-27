using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ConsoleMessageButton : MonoBehaviour
{
    [Header("��������� ���������")]
    [Tooltip("����� ��������� ��� ������ � �������")]
    public string message = "������ ���� ������!";


    private Button button;

    private void Awake()
    {
        // �������� ��������� ������
        button = GetComponent<Button>();

        // ������������� �� ������� �������
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        // ��������� ���������
        string coloredMessage = $"<color=#>{message}</color>";

        // ������� � �������
        Debug.Log(message);
    }


}