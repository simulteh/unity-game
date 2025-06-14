using UnityEngine;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("pizdec");
    }
}
