using UnityEngine;
using UnityEngine.UI;

public class ONC : MonoBehaviour
{
    [Header("Desktop Panels")]
    [SerializeField] GameObject consolePan;
    [SerializeField] GameObject webPan;
    [SerializeField] GameObject settingsPan;
    [SerializeField] GameObject wifiPan;
    [SerializeField] GameObject desktopPan;

    [Header ("Canvases")]
    [SerializeField] GameObject compCanvas;
    [SerializeField] GameObject HUDCanvas;
    [SerializeField] GameObject tabsCanvas;
    [SerializeField] GameObject endScreenCanvas;

    [Header("Tabs")]
    [SerializeField] GameObject[] tabs;
    [SerializeField] GameObject[] tabsPanels;
    int currentTabID = -1;
    //tabMenu;
    //[SerializeField] GameObject tabSettings;
    //[SerializeField] GameObject tabManual;
    //[SerializeField] GameObject tabTablet;

    [Header("Tabs")]
    [SerializeField] GameObject[] appsPanels;

    [Header ("Sprites")]
    [SerializeField] Sprite tabActivedButtonSprite;
    [SerializeField] Sprite tabNotActivatedButtonSprite;

    void Start()
    {
        compCanvas.SetActive(true);
        HUDCanvas.SetActive(true);
        tabsCanvas.SetActive(true);
        consolePan.SetActive(false);
        webPan.SetActive(false);
        settingsPan.SetActive(false);
        desktopPan.SetActive(false);
        wifiPan.SetActive(false);

        SetActivatedTab(-1);
        SetActivateApp(0);
        
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

        if (currentTabID == id)
        {
            currentTabID = -1;
        }
        else
        {
            tabs[id].GetComponent<Image>().sprite = tabActivedButtonSprite;
            tabsPanels[id].SetActive(true);
            currentTabID = id;
        }

    
    }

    public void SetActivateApp(int id)
    {
        for (int i = 0; i < appsPanels.Length; i++)
        {
            appsPanels[i].SetActive(false);
        }

        appsPanels[id].SetActive(true);   
    }

    public void OpenConsolePan()
    {
        if (!consolePan.activeSelf)
        {
            consolePan.SetActive(true);
            consolePan.GetComponent<Console>().InitializeConsole();
        }
        else
        {
            consolePan.SetActive(false);
        }
    }

    public void OpenWifiPan()
    {
        wifiPan.SetActive(!wifiPan.activeSelf);
    }

    public void CloseWifiPan()
    {
        wifiPan.SetActive(false);
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

    public void OpenEndScreenCanvas()
    {
        endScreenCanvas.SetActive(true);
    }

    public void CloseCompCanvas()
    {
        compCanvas.SetActive(false);
    }

    public void OpenCompCanvas()
    {
        compCanvas.SetActive(true);
    }
}

