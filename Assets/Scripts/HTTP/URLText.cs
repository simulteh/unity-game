using System;
using TMPro;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(InputField))]
public class URLText : MonoBehaviour 
{
    
    public string Text;

    private void Start()
    {
        
        TMP_InputField InputField = GetComponent<TMP_InputField>();
        
        InputField.onValueChanged.AddListener(text =>
        {
            this.Text = text;
            print(Text);
        });
    }


    

    
  

}
