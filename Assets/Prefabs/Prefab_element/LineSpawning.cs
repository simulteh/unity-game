using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ObjectConnector : MonoBehaviour, IPointerDownHandler
{
    [Header("Line Settings")]
    public Material lineMaterial;
    public float lineWidth = 0.1f;
    public Color lineColor = Color.blue;
    [Range(0.1f, 2f)] public float colliderWidthMultiplier = 1.5f;

    private static List<GameObject> allLines = new List<GameObject>();
    private static ObjectConnector firstSelected;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Удаление по ПКМ
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Destroy(gameObject);
            // Удаляем все связанные линии
            ClearConnectedLines(gameObject);
            return;
        }

        // Создание соединений по ЛКМ
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (firstSelected == null)
        {
            firstSelected = this;
            return;
        }

        if (firstSelected != this)
        {
            CreateLine(firstSelected.transform, transform);
            firstSelected = null;
        }
    }

    private void CreateLine(Transform start, Transform end)
    {
        GameObject lineObj = new GameObject("ConnectionLine");
        LineRenderer line = lineObj.AddComponent<LineRenderer>();

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = lineMaterial ?? new Material(Shader.Find("Sprites/Default"));
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.positionCount = 2;
        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);

        allLines.Add(lineObj);
    }

    private void ClearConnectedLines(GameObject targetObject)
    {
        List<GameObject> linesToRemove = new List<GameObject>();

        foreach (GameObject line in allLines)
        {
            if (line != null)
            {
                LineRenderer lr = line.GetComponent<LineRenderer>();
                if (lr != null && lr.positionCount == 2)
                {
                    Vector3 startPos = lr.GetPosition(0);
                    Vector3 endPos = lr.GetPosition(1);

                    if (startPos == targetObject.transform.position ||
                        endPos == targetObject.transform.position)
                    {
                        linesToRemove.Add(line);
                        Destroy(line);
                    }
                }
            }
        }

        foreach (GameObject line in linesToRemove)
        {
            allLines.Remove(line);
        }
    }

    public static void ClearAllLines()
    {
        foreach (GameObject line in allLines)
        {
            if (line != null) Destroy(line);
        }
        allLines.Clear();
        firstSelected = null;
    }

    private void Update()
    {
        // Обновление позиций линий (если нужно)
        foreach (GameObject line in allLines)
        {
            if (line != null)
            {
                LineRenderer lr = line.GetComponent<LineRenderer>();
                if (lr != null && lr.positionCount == 2)
                {
                    // Реализация обновления позиций при необходимости
                }
            }
        }
    }
}