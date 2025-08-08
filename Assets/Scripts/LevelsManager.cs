using UnityEngine;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] GameObject infoPanel;

    public void CloseInfoPanel() { infoPanel.SetActive(false); }

    public void OpenInfoPanel() { infoPanel.SetActive(true); }

    public void InverseActiveInfoPanel() { infoPanel.SetActive(!infoPanel.activeSelf); }
}