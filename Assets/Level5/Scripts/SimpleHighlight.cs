using UnityEngine;

public class SimpleHighlight : MonoBehaviour
{
    private Renderer targetRenderer;
    private Color originalColor;
    private bool isHighlighted;
    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null && targetRenderer.material.HasProperty(ColorProp))
            originalColor = targetRenderer.material.color;
    }

    public void SetRenderer(Renderer r)
    {
        targetRenderer = r;
        if (r != null && r.material.HasProperty(ColorProp))
            originalColor = r.material.color;
    }

    public void Highlight()
    {
        if (isHighlighted) return;
        isHighlighted = true;
        if (targetRenderer != null && targetRenderer.material.HasProperty(ColorProp))
        {
            originalColor = targetRenderer.material.color;
            targetRenderer.material.color = Color.Lerp(originalColor, Color.white, 0.3f);
        }
    }

    public void Unhighlight()
    {
        if (!isHighlighted) return;
        isHighlighted = false;
        if (targetRenderer != null && targetRenderer.material.HasProperty(ColorProp))
            targetRenderer.material.color = originalColor;
    }

    private void OnDisable()
    {
        if (isHighlighted)
            Unhighlight();
    }
}
