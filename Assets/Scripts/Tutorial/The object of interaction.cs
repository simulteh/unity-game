using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // Материал, который используется изначально
    private Material originalMaterial;

    // Материал, который подсвечивает объект
    public Material highlightMaterial;

    // Рендерер объекта (присваивается автоматически)
    private Renderer objectRenderer;

    // Чтобы не подсвечивать объект повторно
    private bool isClicked = false;

    // Ссылка на TutorialManager
    private TutorialManager tutorialManager;

    void Start()
    {
        // Получаем рендерер и сохраняем оригинальный материал
        objectRenderer = GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }

        // Получаем ссылку на TutorialManager безопасно
        tutorialManager = Object.FindFirstObjectByType<TutorialManager>();

        if (tutorialManager == null)
        {
            Debug.LogWarning("TutorialManager не найден. Проверь, что он есть на сцене и активен.");
        }
    }

    void OnMouseDown()
    {
        // Если уже нажимали – ничего не делаем
        if (isClicked || tutorialManager == null)
            return;

        isClicked = true;

        // Подсветка объекта
        if (objectRenderer != null && highlightMaterial != null)
        {
            objectRenderer.material = highlightMaterial;
        }

        // Регистрируем выполнение действия
        tutorialManager.RegisterInteraction();
    }
}
