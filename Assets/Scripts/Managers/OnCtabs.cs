using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OnCtabs : MonoBehaviour
{
    [SerializeField] GameObject consolePan;
    [SerializeField] GameObject webPan;
    [SerializeField] GameObject settingsPan;
    [SerializeField] GameObject desktopPan;

    void Start()
    {
        consolePan.SetActive(false);
        webPan.SetActive(false);
        settingsPan.SetActive(false);
        desktopPan.SetActive(false);
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

