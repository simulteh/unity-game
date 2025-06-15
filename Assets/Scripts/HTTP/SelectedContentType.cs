using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectedContentType : MonoBehaviour
{
    public Enum_ContentType contentType;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        switch (name)
        {
            case "TEXT":
                this.contentType = Enum_ContentType.TEXT;
                break;
            case "JSON":
                this.contentType = Enum_ContentType.JSON;
                break;
            case "HTML":
                this.contentType = Enum_ContentType.HTML;
                break;
            case "PNG":
                this.contentType = Enum_ContentType.PNG;
                break;
        }

        print(contentType + " ContentType");


    }

}