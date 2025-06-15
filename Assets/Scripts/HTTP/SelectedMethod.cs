using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectMethod: MonoBehaviour
{
    public Enum_HTTPMethods Method;

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        switch(name)
        {
            case "GET":
                this.Method = Enum_HTTPMethods.GET;
                break;
            case "POST":
                this.Method=Enum_HTTPMethods.POST;
                break;
            case "PUT":
                this.Method=Enum_HTTPMethods.PUT;
                break;
            case "DELETE":
                this.Method=Enum_HTTPMethods.DELETE;
                break;
        }
        print(Method + " Method");




    }

}
