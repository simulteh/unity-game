using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // ��������, ������� ������������ ����������
    private Material originalMaterial;

    // ��������, ������� ������������ ������
    public Material highlightMaterial;

    // �������� ������� (������������� �������������)
    private Renderer objectRenderer;

    // ����� �� ������������ ������ ��������
    private bool isClicked = false;

    // ������ �� TutorialManager
    private TutorialManager tutorialManager;

    void Start()
    {
        // �������� �������� � ��������� ������������ ��������
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }

        // �������� ������ �� TutorialManager ���������
        tutorialManager = Object.FindFirstObjectByType<TutorialManager>();

        if (tutorialManager == null)
        {
            Debug.LogWarning("TutorialManager �� ������. �������, ��� �� ���� �� ����� � �������.");
        }
    }

    void OnMouseDown()
    {
        // ���� ��� �������� � ������ �� ������
        if (isClicked || tutorialManager == null)
            return;

        isClicked = true;

        // ��������� �������
        if (objectRenderer != null && highlightMaterial != null)
        {
            objectRenderer.material = highlightMaterial;
        }

        // ������������ ���������� ��������
        tutorialManager.RegisterInteraction();
    }
}
