using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WebInterface : MonoBehaviour
{
    public int currentID = 0;

    [Header("Panels")]
    [SerializeField] GameObject authPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject workmodePanel;
    [SerializeField] GameObject rangePanel;
    [SerializeField] GameObject dhcpPanel;
    [SerializeField] GameObject ipv4Panel;

    [Header("Search")]
    [SerializeField] TMP_InputField searchInput;

    [Header("Auth")]
    [SerializeField] TMP_InputField loginAuthInput;
    [SerializeField] TMP_InputField passwordAuthInput;
    string loginRouter = "admin";
    string passwordRouter = "admin";
    string passwordAuth = "";
    public bool isAvoid = false;

    [Header("Settings")]
    [SerializeField] TMP_InputField loginSettingsInput;
    [SerializeField] TMP_InputField passwordSettingsInput;
    [SerializeField] TMP_InputField passwordConfirmSettingsInput;

    [Header("Workmode")]
    [SerializeField] Button buttonGateway;
    [SerializeField] Button buttonBridge;
    public bool isGateway = true;

    [Header("Range")]
    [SerializeField] Button buttonSame;
    [SerializeField] Button buttonSwitchWifi_5g;
    [SerializeField] Button buttonSwitchWifi_2g;
    [SerializeField] TMP_InputField nameWifiInput;
    [SerializeField] TMP_InputField passwordWifiInput;
    [SerializeField] TMP_InputField nameWifiInput_2g;
    [SerializeField] TMP_InputField passwordWifiInput_2g;
    public string nameWifi = "";
    public string passwordWifi = "";
    public string nameWifi_2g = "";
    public string passwordWifi_2g = "";
    public bool isSame = false;
    public bool isOn_wifi5g = true;
    public bool isOn_wifi2g = true;

    [Header("DHCP")]
    [SerializeField] Button buttonDHCP;
    [SerializeField] Button buttonManual;
    public bool isDHCP = true;

    [Header("IPV4")]
    [SerializeField] TMP_InputField ipRouterInput;
    [SerializeField] TMP_InputField subnetMaskInput;
    [SerializeField] TMP_InputField ipStartInput;
    [SerializeField] TMP_InputField ipEndInput;
    public string ip = "";
    public string subnet_mask = "";
    public string ipPoolStart = "";
    public string ipPoolFinish = "";


    private void Awake()
    {
        currentID = 0;

        CloseAllPanels();

        searchInput.onEndEdit.AddListener(OnSearchInputEndEdit);

        //Auth
        passwordAuthInput.onValueChanged.AddListener(OnPasswordAuthChange);

        ////Settings
        //adminSettingsInput.onEndEdit.AddListener;
        //passwordSettingsInput.onEndEdit.AddListener;
        //passwordConfirmSettingsInput.onEndEdit.AddListener;

        //Workmode
        buttonGateway.onClick.AddListener(() => ChangeWorkmode("gateway"));
        buttonBridge.onClick.AddListener(() => ChangeWorkmode("bridge"));

        //Range
        buttonSame.onClick.AddListener(SwitchSame);
        buttonSwitchWifi_5g.onClick.AddListener(() => { isOn_wifi5g = !isOn_wifi5g;  });
        buttonSwitchWifi_2g.onClick.AddListener(() => { isOn_wifi2g = !isOn_wifi2g; });

        //DHCP
        buttonDHCP.onClick.AddListener(() => { isDHCP = true;  });
        buttonManual.onClick.AddListener(() => { isDHCP = false;  } );

        //IPV4
        ipRouterInput.onEndEdit.AddListener((string text) => { ip = text;  });
        subnetMaskInput.onEndEdit.AddListener((string text) => { subnet_mask = text; });
        ipStartInput.onEndEdit.AddListener((string text) => { ipPoolStart = text; });
        ipEndInput.onEndEdit.AddListener((string text) => { ipPoolFinish = text; });
    }

    void Start()
    {
        RestartWeb();
    }

    public void RestartWeb()
    {
        currentID = 0;
        CloseAllPanels();
        searchInput.text = "";
    }

    private void OnSearchInputEndEdit(string text)
    {
        string ip_input = text.Trim();

        if (ip_input == "192.168.1.1")
        {
            NextPanel();
        }
        else
        {
            searchInput.text = "";
        }
    }

    private void OnPasswordAuthChange(string text)
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (text == "")
            {
                passwordAuth = "";
            }
            else
            {
                passwordAuth = passwordAuth[0..^1];
            }
        }
        else
        {
            string hide_password = "";

            for (int i = 0; i < text.Length - 1; i++)
            {
                hide_password += "*";
            }
            hide_password += text[^1];
            if (hide_password != passwordAuthInput.text)
            {
                passwordAuthInput.text = hide_password;
                isAvoid = !isAvoid;
            }
            if (!isAvoid && text != "")
            {
                passwordAuth += text[^1];
            }

        }
        Debug.Log(passwordAuth);
    }

    public void EndAuthInput()
    {
        if (loginAuthInput.text.Trim() == loginRouter && passwordAuth.Trim() == passwordRouter)
        {
            currentID += 1;
            NextPanel();
        }
        else
        {
            loginAuthInput.text = "";
            passwordAuthInput.text = "";
        }
    }

    public void EndSettingsInput()
    {
        if (passwordSettingsInput.text != passwordConfirmSettingsInput.text)
        {
            passwordSettingsInput.text = "";
            passwordConfirmSettingsInput.text = "";
            return;
        }
        loginRouter = loginSettingsInput.text;
        passwordRouter = passwordSettingsInput.text;

        currentID += 1;
        NextPanel();
    }

    public void EndWorkmodeInput()
    {
        currentID += 1;
        NextPanel();
    }

    void ChangeWorkmode(string workmode)
    {
        if (workmode == "gateway")
        {
            isGateway = true;
        } else
        {
            isGateway = false;
        }
    }

    public void EndRangeInput()
    {
        currentID += 1;
        NextPanel();
    }

    void SwitchSame()
    {
        isSame = !isSame;
        if (isSame)
        {
            isOn_wifi2g = false;
            buttonSwitchWifi_2g.enabled = false;

            nameWifiInput_2g.gameObject.SetActive(false);
            passwordWifiInput_2g.gameObject.SetActive(false);

        }
    }

    public void EndDHCPInput()
    {
        currentID += 1;
        NextPanel();
    }

    public void EndIPV4Input()
    {
        bool isOk = CheckCorrectnessInput();
        if (!isOk)
        {
            return;
        }
        currentID += 1;
        NextPanel();
    }

    bool CheckCorrectnessInput()
    {

        string[] ip_parts = ip.Split('.');
        for (int i = 0; i < ip_parts.Length; i++)
        {
            if (int.Parse(ip_parts[i]) > 255)
            {
                return false;
            }
        }

        string[] subnetMask_parts = subnet_mask.Split('.');

        for (int i = 0; i < subnetMask_parts.Length; i++)
        {
            if (int.Parse(subnetMask_parts[i]) > 255)
            {
                return false;
            }
        }

        string[] ipPoolStart_parts = ipPoolStart.Split('.');

        for (int i = 0; i < ipPoolStart_parts.Length; i++)
        {
            if (int.Parse(ipPoolStart_parts[i]) > 255)
            {
                return false;
            }
        }

        string[] ipPoolFinish_parts = ipPoolFinish.Split('.');

        for (int i = 0; i < ipPoolFinish_parts.Length; i++)
        {
            if (int.Parse(ipPoolFinish_parts[i]) > 255)
            {
                return false;
            }
        }

        return true;
    }

    public void NextPanel()
    {
        if (currentID == 0)
        {
            CloseAllPanels();
            loginAuthInput.text = "";
            passwordAuthInput.text = "";

            authPanel.SetActive(true);
        }
        else if (currentID == 1)
        {
            CloseAllPanels();
            passwordSettingsInput.text = "";
            passwordConfirmSettingsInput.text = "";

            settingsPanel.SetActive(true);
        }
        else if (currentID == 2)
        {
            CloseAllPanels();
            workmodePanel.SetActive(true);
        }
        else if (currentID == 3)
        {

            if (isSame && isOn_wifi2g)
            {
                nameWifi_2g = nameWifi;
                passwordWifi_2g = passwordWifi;
            }

            CloseAllPanels();
            rangePanel.SetActive(true);
        }
        else if (currentID == 4)
        {
            CloseAllPanels();
            dhcpPanel.SetActive(true);
        }
        else if (currentID == 5)
        {
            CloseAllPanels();
            ipv4Panel.SetActive(true);
        }
        else
        {
            return;
        }
    }

    void CloseAllPanels()
    {
        authPanel.SetActive(false);
        settingsPanel.SetActive(false);
        workmodePanel.SetActive(false);
        rangePanel.SetActive(false);
        dhcpPanel.SetActive(false);
        ipv4Panel.SetActive(false);
    }
}
