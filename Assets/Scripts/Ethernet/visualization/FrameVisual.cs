using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameVisual : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private TextMesh _sizeLabel;

    public void Setup(Color frameColor, int payloadSize)
    {
        _renderer.material.color = frameColor;
        _renderer.transform.localScale = Vector3.one * (0.5f + payloadSize / 1000f);
        _sizeLabel.text = $"{payloadSize} bytes";
    }

    public IEnumerator AnimateMove(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
        Destroy(gameObject);
    }
}