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
    [SerializeField] TMP_InputField nameWifiInput_5g;
    [SerializeField] TMP_InputField passwordWifiInput_5g;
    [SerializeField] TMP_InputField nameWifiInput_2g;
    [SerializeField] TMP_InputField passwordWifiInput_2g;
    public string nameWifi_5g = "";
    public string passwordWifi_5g = "";
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
        buttonSwitchWifi_5g.onClick.AddListener(SwitchWifi5G);
        buttonSwitchWifi_2g.onClick.AddListener(SwitchWifi2G);
        SwitchSame();
        SwitchWifi2G();
        SwitchWifi5G();
        nameWifiInput_5g.onEndEdit.AddListener((string text) => { nameWifi_5g = text; });
        passwordWifiInput_5g.onEndEdit.AddListener((string text) => { passwordWifi_5g = text; });
        nameWifiInput_2g.onEndEdit.AddListener((string text) => { nameWifi_2g = text; });
        passwordWifiInput_2g.onEndEdit.AddListener((string text) => { passwordWifi_2g = text; });

        //DHCP
        buttonDHCP.onClick.AddListener(SwitchDHCPOn);
        buttonManual.onClick.AddListener(SwitchDHCPOn);
        SwitchDHCPOn();

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
            GlobalEvents.Instance.EnterToRouter();

            ShowPanel();
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
        //Debug.Log(passwordAuth);
    }

    public void EndAuthInput()
    {
        if (loginAuthInput.text.Trim() == loginRouter && passwordAuth.Trim() == passwordRouter)
        {
            currentID += 1;
            ShowPanel();
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


        GlobalEvents.Instance.RouterChangeAuthData();

        currentID += 1;
        ShowPanel();
    }

    public void EndWorkmodeInput()
    {
        currentID += 1;
        ShowPanel();
    }

    void ChangeWorkmode(string workmode)
    {
        if (workmode == "gateway")
        {
            isGateway = true;
            buttonGateway.GetComponent<Image>().color = Color.blue;
            buttonBridge.GetComponent<Image>().color = Color.gray;
        } else
        {
            isGateway = false;
            buttonGateway.GetComponent<Image>().color = Color.gray;
            buttonBridge.GetComponent<Image>().color = Color.blue;
        }
    }

    public void EndRangeInput()
    {

        if (isOn_wifi5g && (nameWifiInput_5g.text == "" || passwordWifiInput_5g.text == ""))
        {
            return;
        }

        if (!isSame && isOn_wifi2g && (nameWifiInput_2g.text == "" || passwordWifiInput_2g.text == ""))
        {
            return;
        }

        if (isOn_wifi5g)
        {
            nameWifi_5g = nameWifiInput_5g.text;
            passwordWifi_5g = passwordWifiInput_5g.text;
        }
        if (isSame)
        {
            nameWifi_2g = nameWifi_5g;
            passwordWifi_2g = passwordWifi_5g;
        }
        else if (isOn_wifi2g)
        {
            nameWifi_2g = nameWifiInput_2g.text;
            passwordWifi_2g = passwordWifiInput_2g.text;
        }

        GlobalEvents.Instance.RouterChangeWifiData5g();
        GlobalEvents.Instance.RouterChangeWifiData2g();

        currentID += 1;
        ShowPanel();
    }

    void SwitchSame()
    {
        isSame = !isSame;
        if (isSame)
        {
            buttonSwitchWifi_2g.enabled = false;

            nameWifiInput_2g.gameObject.SetActive(false);
            passwordWifiInput_2g.gameObject.SetActive(false);

            buttonSame.GetComponent<Image>().color = Color.blue;
            buttonSwitchWifi_2g.GetComponent<Image>().color = Color.gray;
        } else
        {
            isOn_wifi2g = true;
            buttonSwitchWifi_2g.enabled = true;

            nameWifiInput_2g.gameObject.SetActive(true);
            passwordWifiInput_2g.gameObject.SetActive(true);

            buttonSame.GetComponent<Image>().color = Color.gray;
            buttonSwitchWifi_2g.GetComponent<Image>().color = Color.gray;
        }
    }

    void SwitchWifi5G()
    {
        isOn_wifi5g = !isOn_wifi5g;

        if (isOn_wifi5g)
        {
            nameWifiInput_5g.gameObject.SetActive(true);
            passwordWifiInput_5g.gameObject.SetActive(true);

            buttonSwitchWifi_5g.GetComponent<Image>().color = Color.blue;
        }
        else
        {
            nameWifiInput_5g.gameObject.SetActive(false);
            passwordWifiInput_5g.gameObject.SetActive(false);

            buttonSwitchWifi_5g.GetComponent<Image>().color = Color.gray;
        }
    }

    void SwitchWifi2G()
    {
        isOn_wifi2g = !isOn_wifi2g;

        if (isOn_wifi2g)
        {
            nameWifiInput_2g.gameObject.SetActive(true);
            passwordWifiInput_2g.gameObject.SetActive(true);

            buttonSwitchWifi_2g.GetComponent<Image>().color = Color.blue;
        }
        else
        {
            nameWifiInput_2g.gameObject.SetActive(false);
            passwordWifiInput_2g.gameObject.SetActive(false);

            buttonSwitchWifi_2g.GetComponent<Image>().color = Color.gray;
        }
    }

    void SwitchDHCPOn()
    {
        isDHCP = !isDHCP;

        if (isDHCP)
        {
            buttonDHCP.GetComponent<Image>().color = Color.blue;
            buttonDHCP.enabled = false;

            buttonManual.GetComponent<Image>().color = Color.gray;
            buttonManual.enabled = true;

        } else
        {
            buttonDHCP.GetComponent<Image>().color = Color.gray;
            buttonDHCP.enabled = true;

            buttonManual.GetComponent<Image>().color = Color.blue;
            buttonManual.enabled = false;
        }
    }

    public void EndDHCPInput()
    {
        currentID += 1;
        ShowPanel();
    }

    public void EndIPV4Input()
    {
        bool isOk = CheckCorrectnessInput(ip);
        if (!isOk)
        {
            return;
        }
        currentID += 1;
        ShowPanel();
    }

    bool CheckCorrectnessInput(string ipAddress)
    {
        if (ipAddress == "")
        {
            return true;
        }

        string[] ip_parts = ipAddress.Split('.');
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

    public void ShowPanel()
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
        else if (currentID == 6)
        {
            CloseAllPanels();
        }
        else
        {
            return;
        }
    }

    public void PrevPanel()
    {
        currentID -= 1;
        ShowPanel();
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
