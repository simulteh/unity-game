using UnityEngine;
using UnityEngine.EventSystems;

public class NetObject : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool isTriggered = false;

    private LineManager LM;

    private RectTransform rectTransform;
    public GameObject dragPanel;
    private Vector2 initialPosition;

    private bool isDrag;

    void Awake()
    {
        isDrag = false;

        rectTransform = GetComponent<RectTransform>();

        LM = GameObject.FindGameObjectWithTag("LineManager").GetComponent<LineManager>();

        isTriggered = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            LM.RemoveLinesForObject(gameObject);
            Destroy(gameObject);
            //Debug.Log("Удалил");
            return;
        }

        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        initialPosition = rectTransform.anchoredPosition;
        //Debug.Log("Клик");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Drag");

        isDrag = true;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            dragPanel.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Подняли");
        if (GetComponent<NetObject>().isTriggered)
        {
            rectTransform.anchoredPosition = initialPosition;
        }
        else
        {
            if (isDrag)
            {
                LM.UpdateLinePositions(this.gameObject);
            } else
            {
                LM.Select(this.gameObject);
            }
        }

        isDrag = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("+");
        isTriggered = true;
    }
    
    private void OnTriggerExit2D(Collider2D other) {
        //Debug.Log("-");
        isTriggered = false;
    }
}