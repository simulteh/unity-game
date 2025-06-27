using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class CanvasEdgeSpawner : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Spawn Settings")]
    public GameObject spawnPrefab; // Префаб для создания
    public float minDistanceFromEdge = 50f; // Минимальное расстояние от краев Canvas

    [Header("Visual Feedback")]
    [Range(0.1f, 0.9f)] public float dragAlpha = 0.6f; // Прозрачность при перетаскивании
    public Color validPositionColor = Color.white;
    public Color invalidPositionColor = Color.red;

    private GameObject draggedObject;
    private RectTransform canvasRect;
    private Canvas parentCanvas;
    private Image draggedImage;
    private bool isCreatingNewObject = false;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            canvasRect = parentCanvas.GetComponent<RectTransform>();
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || spawnPrefab == null)
            return;



        // Проверяем, не нажали ли мы на уже существующий объект
        if (eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject != gameObject &&
            eventData.pointerCurrentRaycast.gameObject.GetComponent<DraggableUIElement>() != null)
        {
            // Нажали на существующий объект - начинаем его перемещение
            draggedObject = eventData.pointerCurrentRaycast.gameObject;
            isCreatingNewObject = false;

            if (draggedObject.TryGetComponent(out draggedImage))
            {
                draggedImage.color = new Color(validPositionColor.r, validPositionColor.g, validPositionColor.b, dragAlpha);
            }
        }
        else
        {
            // Создаем новый объект
            draggedObject = Instantiate(spawnPrefab, parentCanvas.transform);
            draggedObject.transform.position = transform.position;

            // Добавляем компонент для перемещения
            if (!draggedObject.GetComponent<DraggableUIElement>())
            {
                draggedObject.AddComponent<DraggableUIElement>();
            }

            isCreatingNewObject = true;

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject == null) return;

        // Конвертируем позицию мыши в локальные координаты Canvas
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // Для новых объектов проверяем границы
            bool isValidPosition = true;
            if (isCreatingNewObject)
            {
                isValidPosition = IsPositionValid(localPoint);
            }

            // Обновляем позицию
            draggedObject.GetComponent<RectTransform>().anchoredPosition = localPoint;



        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject == null) return;

        // Проверяем валидность финальной позиции (только для новых объектов)
        if (isCreatingNewObject)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            {
                if (!IsPositionValid(localPoint))
                {
                    // Уничтожаем объект если позиция невалидна
                    Destroy(draggedObject);
                }
                else
                {
                    // Фиксируем объект
                    if (draggedImage != null)
                    {
                        draggedImage.color = validPositionColor;
                    }
                }
            }
        }
        else
        {
            // Для перемещаемого объекта просто сбрасываем прозрачность
            if (draggedImage != null)
            {
                draggedImage.color = validPositionColor;
            }
        }

        draggedObject = null;
        draggedImage = null;
        isCreatingNewObject = false;
    }

    private bool IsPositionValid(Vector2 position)
    {
        if (canvasRect == null) return true;

        // Рассчитываем границы с учетом минимального расстояния
        float minX = canvasRect.rect.xMin + minDistanceFromEdge;
        float maxX = canvasRect.rect.xMax - minDistanceFromEdge;
        float minY = canvasRect.rect.yMin + minDistanceFromEdge;
        float maxY = canvasRect.rect.yMax - minDistanceFromEdge;

        return position.x >= minX && position.x <= maxX &&
               position.y >= minY && position.y <= maxY;
    }

    // Визуализация границ в редакторе
    private void OnDrawGizmosSelected()
    {
        if (canvasRect == null) return;

        Vector3[] corners = new Vector3[4];
        canvasRect.GetWorldCorners(corners);

        // Внешние границы Canvas
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(corners[0], corners[1]);
        Gizmos.DrawLine(corners[1], corners[2]);
        Gizmos.DrawLine(corners[2], corners[3]);
        Gizmos.DrawLine(corners[3], corners[0]);

        // Внутренние границы (с учетом минимального расстояния)
        float margin = minDistanceFromEdge * canvasRect.lossyScale.x;
        Vector3 innerMin = corners[0] + new Vector3(margin, margin, 0);
        Vector3 innerMax = corners[2] - new Vector3(margin, margin, 0);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(innerMin.x, innerMin.y), new Vector3(innerMax.x, innerMin.y));
        Gizmos.DrawLine(new Vector3(innerMax.x, innerMin.y), new Vector3(innerMax.x, innerMax.y));
        Gizmos.DrawLine(new Vector3(innerMax.x, innerMax.y), new Vector3(innerMin.x, innerMax.y));
        Gizmos.DrawLine(new Vector3(innerMin.x, innerMax.y), new Vector3(innerMin.x, innerMin.y));
    }
}

// Новый класс для перемещения объектов
public class DraggableUIElement : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;
    private Image image;
    private Color originalColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentCanvas = GetComponentInParent<Canvas>();

        if (TryGetComponent(out image))
        {
            originalColor = image.color;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (image != null)
        {
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.6f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (image != null)
        {
            image.color = originalColor;
        }
    }
}