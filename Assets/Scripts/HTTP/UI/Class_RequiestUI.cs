using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;




public class Class_RequiestUI : MonoBehaviour
{
    public Enum_HTTPMethods HTTPMethod = Enum_HTTPMethods.NULL;
    public Enum_ContentType ContentType = Enum_ContentType.NULL;
    
    [SerializeField] private TMP_InputField inputField;
    public string URL;
    public TMP_Text StatusText;
    

    public void Start()  //��������� �������
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(ChangeURLText);
    }

  
    public void ChangeContentType(string name) //��������� ���� ��������
    {
        switch (name)
        {
            case "TEXT":
                this.ContentType = Enum_ContentType.TEXT;
                break;
            case "JSON":
                this.ContentType = Enum_ContentType.JSON;
                break;
            case "HTML":
                this.ContentType = Enum_ContentType.HTML;
                break;
            case "PNG":
                this.ContentType = Enum_ContentType.PNG;
                break;
        }

        print(ContentType + " ContentType");
    }

    public void ChangeURLText(string text) //��������� ���
    {
       URL = text;
    }
    public void ChangeMethod(string name) //��������� ������
    {
        switch (name)
        {
            case "GET":
                this.HTTPMethod = Enum_HTTPMethods.GET;
                break;
            case "POST":
                this.HTTPMethod = Enum_HTTPMethods.POST;
                break;
            case "PUT":
                this.HTTPMethod = Enum_HTTPMethods.PUT;
                break;
            case "DELETE":
                this.HTTPMethod = Enum_HTTPMethods.DELETE;
                break;
        }

        print(HTTPMethod + " METHOD");
    }

    public void SendRequest() //�������� �������
    {
        if ((URL != null && URL != "") && (HTTPMethod != Enum_HTTPMethods.NULL) && (ContentType != Enum_ContentType.NULL))
        {
            StatusText.text = "������ ������� ���������";
            StatusText.color = Color.green;
        }
        else
        {
            StatusText.text = "������. ���������� ��������� ������";
            StatusText.color = Color.red;
            //����� � �������
        }
        Debug.LogWarning("INFO");
        Debug.LogWarning(URL + " -- URL");
        Debug.LogWarning(ContentType + " -- ContentType");
        Debug.LogWarning(HTTPMethod + " -- HTTPMethod");
    }
}





