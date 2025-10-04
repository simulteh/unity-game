using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class CanvasEdgeSpawner : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Spawn Settings")]
    public GameObject netObjectPrefab; 
    public float minDistanceFromEdge = 50f; 

    private GameObject draggedObject;
    private RectTransform canvasRect;


    [SerializeField] public GameObject dragPanel;

    private bool isCreatingNewObject = false;

    private void Awake()
    {
        canvasRect = dragPanel.GetComponent<RectTransform>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {        
        if (eventData.button != PointerEventData.InputButton.Left || netObjectPrefab == null)
            return;
   
        if (eventData.pointerCurrentRaycast.gameObject != null &&
            eventData.pointerCurrentRaycast.gameObject != gameObject &&
            eventData.pointerCurrentRaycast.gameObject.GetComponent<NetObject>() != null)
        {
            draggedObject = eventData.pointerCurrentRaycast.gameObject;
            isCreatingNewObject = false;
        }
        else
        {
            draggedObject = Instantiate(netObjectPrefab, dragPanel.transform);
            draggedObject.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
            draggedObject.transform.position = transform.position;

            if (!draggedObject.GetComponent<NetObject>())
            {
                draggedObject.AddComponent<NetObject>();
            }
            draggedObject.GetComponent<NetObject>().dragPanel = this.dragPanel;

            isCreatingNewObject = true;

        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedObject == null) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {

            draggedObject.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject == null) return;

        if (isCreatingNewObject)
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            {
                if (!IsPositionValid())
                {
                    Destroy(draggedObject);
                }
            }
        }

        draggedObject = null;
        isCreatingNewObject = false;
    }

    private bool IsPositionValid()
    {
        if (draggedObject.GetComponent<NetObject>().isTriggered)
        {
            return false;
        }

        return true;
    }


}


