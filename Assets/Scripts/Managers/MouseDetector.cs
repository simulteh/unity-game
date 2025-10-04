using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseDetector : MonoBehaviour
{
    public GameObject target = null;
    GameObject prevTarget = null;

    [Header("UI")]
    [SerializeField] ONC ONC;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] TMP_InputField inputTargetName;

    private float lastClickTime;
    public float doubleClickTimeThreshold = 0.3f; // Time in seconds to register a double-click

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                prevTarget = target;
                target = clickedObject;

                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTimeThreshold)
                {
                    DoubleClick();
                }
                else
                {
                    SingleClick();
                }

                // Update last click time
                lastClickTime = Time.time;

                
            }
        }
    }

    private void OnTargetNameChanged(string text)
    {
        target.name = inputTargetName.text;
    }

    void SingleClick()
    {
        //GUI
        //infoPanel.SetInfo(clickedObject);
        inputTargetName.text = target.name;
        inputTargetName.onValueChanged.AddListener(OnTargetNameChanged);
        //ONC.OpenTargetPanel();

        if (prevTarget) prevTarget.GetComponent<Outline>().enabled = false;
        target.GetComponent<Outline>().enabled = true;
    }

    void DoubleClick()
    {
        if (target.name == "PC")
        {
            ONC.OpenDesktopPan();
            ONC.OpenCompCanvas();
        }
    }
}
