using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public Vector2 start;
    public Vector2 end;
    public float thickness = 5f;


    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Vector2 direction = end - start;
        //float length = direction.magnitude;
        Vector2 perpendicular = Vector2.Perpendicular(direction).normalized * thickness / 2;

        // Calculate vertices
        Vector2 v1 = start + perpendicular;
        Vector2 v2 = start - perpendicular;
        Vector2 v3 = end - perpendicular;
        Vector2 v4 = end + perpendicular;

        // Create quad
        vh.AddVert(v1, color, new Vector2(0, 0));
        vh.AddVert(v2, color, new Vector2(0, 1));
        vh.AddVert(v3, color, new Vector2(1, 1));
        vh.AddVert(v4, color, new Vector2(1, 0));

        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }
}

