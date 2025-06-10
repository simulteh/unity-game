using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseDetector : MonoBehaviour
{
    public GameObject target = null;
    GameObject prevTarget = null;

    [Header("UI")]
    [SerializeField] GUI gui;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] TMP_InputField inputTargetName;

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

                //GUI
                //infoPanel.SetInfo(clickedObject);
                inputTargetName.text = clickedObject.name;
                inputTargetName.onValueChanged.AddListener(OnTargetNameChanged);
                gui.OpenGUI();
                gui.OpenTargetPanel();

                if (prevTarget) prevTarget.GetComponent<Outline>().enabled = false;
                target.GetComponent<Outline>().enabled = true;
            }
        }
    }

    private void OnTargetNameChanged(string text)
    {
        target.name = inputTargetName.text;
    }

}
