using UnityEngine;
using UnityEngine.UI;

public class TerminalAutoScroll : MonoBehaviour
{
    private ScrollRect scrollRect;

    void Start() => scrollRect = GetComponentInParent<ScrollRect>();

    void LateUpdate()
    {
        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }
}