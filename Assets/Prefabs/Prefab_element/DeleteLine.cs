using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LineCleaner : MonoBehaviour
{
    private Button cleanButton;

    private void Awake()
    {
        // �������� ��������� ������
        cleanButton = GetComponent<Button>();

        // ������������� �� ������� �������
        cleanButton.onClick.AddListener(CleanAllLines);
    }

    public void CleanAllLines()
    {
        // ������� ��� ����� � �����
        LineRenderer[] allLines = FindObjectsOfType<LineRenderer>();

        // ���������� ������ ������ � LineRenderer
        foreach (LineRenderer line in allLines)
        {
            if (line.gameObject.name == "ConnectionLine")
            {
                Destroy(line.gameObject);
            }
        }

        Debug.Log("��� ����� �������!");
    }

    private void OnDestroy()
    {
        // ������������ �� ������� ��� ����������� �������
        if (cleanButton != null)
        {
            cleanButton.onClick.RemoveListener(CleanAllLines);
        }
    }
}