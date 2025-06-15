
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(TMP_Text))]

public class Class_GUI : MonoBehaviour
{
    SelectMethod selectMethod;
    SelectedContentType selectedContentType;
    URLText URLText;
    
    
   
    public void Start()
    {
        Button RequestButton; RequestButton = GetComponent<Button>();
        TMP_Text StatusText; StatusText = GetComponent<TMP_Text>();

        this.selectMethod = gameObject.AddComponent<SelectMethod>();
        this.selectedContentType = gameObject.AddComponent<SelectedContentType>();
        this.URLText = gameObject.AddComponent<URLText>();



        RequestButton.onClick.AddListener(() =>
        {
            if ((this.selectMethod != null) && (this.selectedContentType != null) && (this.URLText != null))
            {
                StatusText.text = "Запрос Отправлен";
            }
            else
            {
                StatusText.text = "Ошибка, чего то не хватает";
            }
        });
    }


    
  
}





