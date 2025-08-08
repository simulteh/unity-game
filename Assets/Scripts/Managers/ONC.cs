using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ONC : MonoBehaviour
{
    [SerializeField] GameObject consolePan;
    [SerializeField] GameObject webPan;
    [SerializeField] GameObject settingsPan;
    [SerializeField] GameObject desktopPan;

    [Header ("Canvases")]
    [SerializeField] GameObject compCanvas;
    [SerializeField] GameObject HUDCanvas;
    [SerializeField] GameObject panelsCanvas;

    [Header("Tabs")]
    [SerializeField] GameObject[] tabs;
    [SerializeField] GameObject[] tabsPanels;
    int currentID = -1;
    //tabMenu;
    //[SerializeField] GameObject tabSettings;
    //[SerializeField] GameObject tabManual;
    //[SerializeField] GameObject tabTablet;

    [Header ("Sprites")]
    [SerializeField] Sprite tabActivedButtonSprite;
    [SerializeField] Sprite tabNotActivatedButtonSprite;

    void Start()
    {
        compCanvas.SetActive(true);
        HUDCanvas.SetActive(true);
        panelsCanvas.SetActive(true);
        consolePan.SetActive(false);
        webPan.SetActive(false);
        settingsPan.SetActive(false);
        desktopPan.SetActive(false);

        SetActivatedTab(-1);
        
    }

    public void SetActivatedTab(int id)
    {
        //tabMenu.GetComponent<Image>().sprite = tabNotActivatedButtonSprite;
        //tabSettings.GetComponent<Image>().sprite = tabNotActivatedButtonSprite;
        //tabManual.GetComponent<Image>().sprite = tabNotActivatedButtonSprite;
        //tabTablet.GetComponent<Image>().sprite = tabNotActivatedButtonSprite;
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].GetComponent<Image>().sprite = tabNotActivatedButtonSprite;
            tabsPanels[i].SetActive(false);
        }

        if (currentID == id)
        {
            currentID = -1;
        }
        else
        {
            tabs[id].GetComponent<Image>().sprite = tabActivedButtonSprite;
            tabsPanels[id].SetActive(true);
            currentID = id;
        }

    
    }

    public void OpenConsolePan()
    {
        if (!consolePan.activeSelf)
        {
            consolePan.SetActive(true);
            consolePan.GetComponent<Console>().RestartConsole();
        }
        else
        {
            consolePan.SetActive(false);
        }
    }

    public void CloseConsolePan()
    {
        consolePan.SetActive(false);
    }

    public void OpenWebPan()
    {
        webPan.SetActive(true);
    }

    public void CloseWebPan()
    {
        webPan.SetActive(false);
    }

    public void OpenSettingsPan()
    {
        settingsPan.SetActive(true);
    }

    public void CloseSettingsPan()
    {
        settingsPan.SetActive(false);
    }

    public void OpenDesktopPan()
    {
        desktopPan.SetActive(true);
    }

    public void CloseDesktopPan()
    {
        desktopPan.SetActive(false);
    }
}

