using UnityEngine;

public class ConnectionVisualizer : MonoBehaviour
{
    public Router source;
    public Router target;
    private LineRenderer line;

    void Start()
    {
        line = gameObject.AddComponent<LineRenderer>();

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = new Material(Shader.Find("Standard"));
        line.material.color = Color.blue;

        line.positionCount = 2;
    }

    void Update()
    {
        if (source != null && target != null)
        {
            line.SetPosition(0, source.transform.position);
            line.SetPosition(1, target.transform.position);
        }
    }
}