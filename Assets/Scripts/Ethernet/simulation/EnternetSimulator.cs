using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class EthernetSimulator : MonoBehaviour
{
    public static EthernetSimulator Instance;
    public List<EthernetInterface> Interfaces = new();

    void Awake() => Instance = this;

    // Передача кадра между устройствами
    public void TransmitFrame(EthernetFrame frame, EthernetInterface sender)
    {
        foreach (var iface in Interfaces.Where(i => i != sender))
        {
            StartCoroutine(DeliverFrame(frame, iface));
        }
    }

    IEnumerator DeliverFrame(EthernetFrame frame, EthernetInterface receiver)
    {
        yield return new WaitForSeconds(0.1f); // Задержка передачи
        receiver.ReceiveFrame(frame);
    }
}