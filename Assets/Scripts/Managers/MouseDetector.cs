using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DetectionEventArgs
{
    public GameObject Target;
    public GameObject? PrevTarget;

    public DetectionEventArgs(GameObject target, GameObject prevTarget)
    {
        Target = target;
        PrevTarget = prevTarget;
    }
}
public class MouseDetector : MonoBehaviour
{
    public GameObject target = null;
    GameObject prevTarget = null;

    [Header("UI")]
    [SerializeField] GUI gui;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] TMP_InputField inputTargetName;
    public delegate void DetectionHandler(object sender, DetectionEventArgs args);
    public event DetectionHandler OnDetection;

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
                OnDetection.Invoke(this, new DetectionEventArgs(target, prevTarget));
            }
        }
    }

    private void OnTargetNameChanged(string text)
    {
        target.name = inputTargetName.text;
    }

}
