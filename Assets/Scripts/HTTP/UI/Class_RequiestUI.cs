using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_InputField))]


public class Class_RequiestUI : MonoBehaviour
{
    Enum_HTTPMethods HTTPMethod = Enum_HTTPMethods.NULL;
    Enum_ContentType ContentType = Enum_ContentType.NULL;

    public string URL;
    public TMP_Text StatusText;

    private void Start() //��������� �������
    {
        TMP_InputField InputURL = GetComponent<TMP_InputField>();
        InputURL.onEndEdit.AddListener(ChangeURLText);
        
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

    private void ChangeURLText(string text) //��������� ���
    {
       this.URL = text;
       print(URL); 
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
        if ((this.URL != null && this.URL != "") && (HTTPMethod != Enum_HTTPMethods.NULL) && (ContentType != Enum_ContentType.NULL))
        {
            StatusText.text = "������ ������� ���������";
            StatusText.color = Color.green;
        }
        else
        {
            StatusText.text = "������. ���������� ��������� ������";
            StatusText.color = Color.red;
            //����� � �������
            print("INFO");
            print(URL);
            print(ContentType);
            print(HTTPMethod);
           
            
            
        }

        
    }


    
  
}





