using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AdminTabletUI : MonoBehaviour
{
    public GameObject deviceListContainer;
    public GameObject deviceUIPrefab;

    public void LoadDevices(List<NetworkDevice> devices)
    {
        foreach (Transform child in deviceListContainer.transform)
            GameObject.Destroy(child.gameObject);

        foreach (var device in devices)
        {
            GameObject ui = Instantiate(deviceUIPrefab, deviceListContainer.transform);
            Text deviceText = ui.GetComponentInChildren<Text>();
            Button editButton = ui.transform.Find("EditButton").GetComponent<Button>();
            Image statusIndicator = ui.transform.Find("StatusIndicator").GetComponent<Image>();

            deviceText.text = device.Name + " (" + device.IP + ")";
            statusIndicator.color = device.IsConfiguredProperly() ? Color.green : Color.red;

            editButton.onClick.AddListener(() =>
            {
                // Примерная логика (должна быть заменена UI-панелью настройки)
                string newIP = PromptForIP(device.IP); // реализуйте как хотите
                if (!string.IsNullOrEmpty(newIP))
                {
                    device.IP = newIP;
                    deviceText.text = device.Name + " (" + device.IP + ")";
                    statusIndicator.color = device.IsConfiguredProperly() ? Color.green : Color.red;
                }
            });
        }
    }

    private string PromptForIP(string currentIP)
    {
        // Здесь может быть вызов UI Input Field
        return UnityEngine.Random.Range(1, 255).ToString() + ".168.1.100"; // Пример
    }
}
