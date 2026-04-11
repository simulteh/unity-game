using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI targetName;
    [SerializeField] TMP_InputField inputField_targetName;
    [SerializeField] TMP_InputField inputField_targetIP;
    [SerializeField] TextMeshProUGUI targetMAC;

    GameObject target;

    private void Awake()
    {
        this.gameObject.SetActive(false);
    }
    
    void Start()
    {
        inputField_targetName.text = "";
        inputField_targetIP.text = "0.0.0.0";
    }

    public void RenameTargetName()
    {
        //name
        target.name = inputField_targetName.text;
        targetName.text = inputField_targetName.text;
    }

    public void SetInfo(GameObject clickedObject)
    {
        target = clickedObject;
        // top name
        targetName.text = target.name;
        // input name
        inputField_targetName.text = target.name;
        // ip
        inputField_targetIP.text = target.GetComponent<IpConfig>().ip;
        // mac
        targetMAC.text = target.GetComponent<MAC>().MACAddres;
    }
}
