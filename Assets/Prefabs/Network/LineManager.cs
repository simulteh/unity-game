using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LineManager : MonoBehaviour
{
    [SerializeField] GameObject DragPanel;

    [Header("Line Settings")]
    [Range(1f, 10f)] public float lineThickness = 5f;
    public Color lineColor = Color.black;

    public GameObject firstSelected;

    private Dictionary<GameObject, List<GameObject>> objectLineMap = new();
    private Dictionary<GameObject, (GameObject, GameObject)> lineObjectMap = new();

    public void Select(GameObject gm)
    {
        if (firstSelected == null)
        {
            firstSelected = gm;
            return;
        }

        if (firstSelected != this)
        {
            CreateLine(firstSelected, gm, firstSelected.GetComponent<RectTransform>(), gm.GetComponent<RectTransform>());
            firstSelected = null;
        }
    }

    private void CreateLine(GameObject startObject, GameObject endObject, RectTransform start, RectTransform end)
    {
        GameObject lineObj = new GameObject("ConnectionLine", typeof(RectTransform));
        lineObj.transform.SetParent(DragPanel.transform, false);
        lineObj.AddComponent<CanvasRenderer>();

        UILineRenderer line = lineObj.AddComponent<UILineRenderer>();
        line.start = start.anchoredPosition;
        line.end = end.anchoredPosition;
        line.thickness = lineThickness;
        line.color = lineColor;

        // Добавляем линию в список для каждого объекта
        AddLineToObject(startObject, lineObj);
        AddLineToObject(endObject, lineObj);

        // Сохраняем информацию о соединенных объектах для линии
        lineObjectMap[lineObj] = (startObject, endObject);

    }

    private void AddLineToObject(GameObject obj, GameObject line)
    {
        if (!objectLineMap.ContainsKey(obj))
        {
            objectLineMap[obj] = new List<GameObject>();
        }
        objectLineMap[obj].Add(line);
    }

    public void UpdateLinePositions(GameObject movedObject)
    {
        
        if (!objectLineMap.ContainsKey(movedObject)) return;

        foreach (GameObject lineObj in objectLineMap[movedObject])
        {
            
            UILineRenderer line = lineObj.GetComponent<UILineRenderer>();

            // Извлекаем объекты, соединяемые этой линией
            var (startObject, endObject) = lineObjectMap[lineObj];

            RectTransform startRectTransform = startObject.GetComponent<RectTransform>();
            RectTransform endRectTransform = endObject.GetComponent<RectTransform>();

            if (startRectTransform != null && endRectTransform != null)
            {
                
                
                line.start = startRectTransform.anchoredPosition;
                line.end = endRectTransform.anchoredPosition;
                
                
            }

            line.SetVerticesDirty();
        }
    }

    // очистка всего поля
    public void RemoveAllLinesAndObjects()
    {
        foreach (var item in objectLineMap)
        {
            foreach (var line in item.Value)
            {
                Destroy(line);
            }
            Destroy(item.Key);
        }
        objectLineMap.Clear();
    }

    // Удаление всех линий
    public void RemoveAllLines()
    {
        foreach (var item in objectLineMap)
        {
            foreach (var line in item.Value)
            {
                Destroy(line);
            }
        }

        foreach (var key in objectLineMap.Keys)
        {
            objectLineMap[key].Clear();
        }
    }

    // Удаление объекта и его линий
    public void RemoveLinesForObject(GameObject obj)
    {
        if (objectLineMap.ContainsKey(obj))
        {
            foreach (var line in objectLineMap[obj])
            {
                Destroy(line);
            }
            objectLineMap.Remove(obj);
        }
    }

    // удаление одной линии
    public void RemoveSingleLine(GameObject lineObj)
    {
        foreach (var item in objectLineMap)
        {
            if (item.Value.Remove(lineObj))
            {
                Destroy(lineObj);
                // Прерывание цикла после удаления линии из всех объектов
                break;
            }
        }
    }

}
