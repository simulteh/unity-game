using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Console : MonoBehaviour
{
    TMP_InputField consoleInputField;

    const string default_text = "C:\\Users\\User1> ";
    string previous_text;
    int minLength;

    [SerializeField] MouseDetector mouseDetector;

    private void Awake()
    {
        consoleInputField = GetComponent<TMP_InputField>();
    }

    public void RestartConsole()
    {
        minLength = default_text.Length;
        previous_text = default_text;
        consoleInputField.text = default_text;

        consoleInputField.Select();
        consoleInputField.caretPosition = consoleInputField.text.Length;

        consoleInputField.onValueChanged.AddListener(OnInputValueChanged);
        consoleInputField.onEndEdit.AddListener(OnInputEndEdit);
    }

    private void OnInputValueChanged(string newText)
    {
        if (newText.Length < minLength)
        {
            consoleInputField.text = previous_text;
            consoleInputField.MoveTextEnd(false);
        }
        consoleInputField.caretPosition = newText.Length;
    }

    private void OnInputEndEdit(string text)
    {
        string command = text.Replace(previous_text, "").Trim();

        if (command == "ipconfig")
        {
            TextForIpConfig();
        }
        else if (command == "getmac")
        {
            TextForGetMac();
        }
        else if (command == "clear")
        {
            RestartConsole();
            return;
        } else
        {
            return;
        }

        consoleInputField.text += "\n" + default_text;

        previous_text = consoleInputField.text;
        minLength = previous_text.Length;

        consoleInputField.MoveTextEnd(false);
        consoleInputField.ActivateInputField();
    }

    private void TextForIpConfig()
    {
        IpConfig ipconfig = mouseDetector.target.GetComponent<IpConfig>();
        consoleInputField.text += "\nEthernet adapter Ethernet:\n\n";
        consoleInputField.text += "\tConnection - specific DNS Suffix  :\n";
        consoleInputField.text += "\tLink - local IPv6 Address . . . . : fe80::e5d8:c103: af14: c37b % 11\n";
        consoleInputField.text += $"\tIPv4 Address. . . . . . . . . . . : {ipconfig.ip}\n";
        consoleInputField.text += $"\tSubnet Mask . . . . . . . . . . . : {ipconfig.subnetMask}\n";
        consoleInputField.text += $"\tDefault Gateway . . . . . . . . . : {ipconfig.gateway}";
    }

    private void TextForGetMac()
    {
        MAC mac = mouseDetector.target.GetComponent<MAC>();
        consoleInputField.text += "\n\n";
        consoleInputField.text += "Phisycal Addres \tTransport Name\n";
        consoleInputField.text += "=============\t======================================\n";
        consoleInputField.text += $"{mac.MACAddres}\t () \n";
    }
}
