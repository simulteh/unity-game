using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Connections : MonoBehaviour
{
    [SerializeField] WebInterface webInterface;

    [SerializeField] GameObject ConnectionPrefab;

    public List<GameObject> connectionsGO = new();

    public void UpdateConnections()
    {
        foreach (var con in connectionsGO)
        {
            Destroy(con);
        }

        connectionsGO.Clear();

        if (webInterface.nameWifi_5g != "")
        {
            GameObject newConnect = Instantiate(ConnectionPrefab, gameObject.transform);
            connectionsGO.Add(newConnect);

            newConnect.GetComponentInChildren<TextMeshProUGUI>().text = webInterface.nameWifi_5g;
            newConnect.GetComponent<Button>().onClick.AddListener(() => {GlobalEvents.Instance.ComputerConnectedRouter(); });
        }

        if (webInterface.nameWifi_2g != "")
        {
            GameObject newConnect = Instantiate(ConnectionPrefab, gameObject.transform);
            connectionsGO.Add(newConnect);

            newConnect.GetComponentInChildren<TextMeshProUGUI>().text = webInterface.nameWifi_2g;
            newConnect.GetComponent<Button>().onClick.AddListener(() => { GlobalEvents.Instance.ComputerConnectedRouter(); });
        }
    }
}
