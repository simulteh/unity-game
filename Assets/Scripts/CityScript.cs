using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityScript : MonoBehaviour
{
    [SerializeField] GameObject infoPanel;

    public void OpenInfoPanel()
    {
        infoPanel.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(true);
    }
}
