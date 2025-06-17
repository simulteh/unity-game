using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] GameObject GUI_canvas;
    [SerializeField] GameObject tabs;
    [SerializeField] GameObject target;
    [SerializeField] GameObject console;

    private void Start()
    {
        CloseGUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GUI_canvas.activeSelf)
            {
                CloseGUI();
            }
            else
            {
                OpenGUI();
            }
        }
    }

    public void CloseGUI()
    {
        GUI_canvas.SetActive(false);
    }

    public void OpenGUI()
    {
        GUI_canvas.SetActive(true);
        tabs.SetActive(true);
        target.SetActive(true);

        console.SetActive(false);
    }

    public void OpenTargetPanel()
    {
        target.SetActive(true);
    }
}
