using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;


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
        else if (command.StartsWith("ping"))
        {
            string pattern = @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$";
            string ip = command.Split()[1];

            if (Regex.IsMatch(ip, pattern))
            {
                Debug.Log($"{ip} is a valid IPv4 address.");
            }
            else
            {
                TextForInvalidIp();
                //Debug.Log($"{ip} is not a valid IPv4 address.");
            }
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

    private void TextForInvalidIp()
    {
        consoleInputField.text += "\n\n";
        consoleInputField.text += $"Error - is not a valid IPv4 address.\n";
    }
}
