using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [Tooltip("UI панель, которая откроется при входе")]
    public GameObject uiPanelToOpen;
    [Tooltip("Название шага для прогресса (оставьте пустым, если не нужно)")]
    public string stepToActivate;

    private void OnTriggerEnter(Collider other)
    {
        if (uiPanelToOpen != null) uiPanelToOpen.SetActive(true);
        if (!string.IsNullOrEmpty(stepToActivate))
            Level5Manager.Instance.SetStep(stepToActivate);
    }

    private void OnTriggerExit(Collider other)
    {
        if (uiPanelToOpen != null) uiPanelToOpen.SetActive(false);
    }
}