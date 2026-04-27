using UnityEngine;
public class DebugUIToggle : MonoBehaviour
{
    public GameObject routerPanel, laptopPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            routerPanel.SetActive(!routerPanel.activeSelf);
        if (Input.GetKeyDown(KeyCode.L))
            laptopPanel.SetActive(!laptopPanel.activeSelf);
    }
}