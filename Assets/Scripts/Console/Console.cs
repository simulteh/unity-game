using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class Console : MonoBehaviour
{
    TMP_InputField consoleInputField;

    const string commandPrompt = "C:\\Users\\User1>";
    string protectedConsoleContent;
    bool isSettingTextProgrammatically = false;

    [SerializeField] MouseDetector mouseDetector;

    private bool isInitialized = false; // Флаг для отслеживания инициализации

    private void Awake()
    {
        //Debug.Log("Awake");
        consoleInputField = GetComponent<TMP_InputField>();
        
    }

    private void OnEnable()
    {
        if (!isInitialized)
        {
            InitializeConsole();
        }
        else if (consoleInputField != null)
        {
            // При повторном открытии просто активируем поле ввода
            consoleInputField.ActivateInputField();
            SetCaretToEnd();
        }
    }


    private void OnDisable()
    {
        // Безопасная деактивация
        if (consoleInputField != null)
        {
            consoleInputField.DeactivateInputField();
        }
    }

    private void OnDestroy()
    {
        // Очищаем обработчики при уничтожении
        if (consoleInputField != null)
        {
            consoleInputField.onValueChanged.RemoveAllListeners();
            consoleInputField.onSubmit.RemoveAllListeners();
        }
    }

    //private void Start()
    //{
    //    //Debug.Log("Start");

    //    // Убедимся, что удаляем старые обработчики перед добавлением новых
    //    consoleInputField.onValueChanged.RemoveAllListeners();
    //    consoleInputField.onSubmit.RemoveAllListeners();

    //    InitializeConsole();
    //}

    public void InitializeConsole()
    {
        if (isInitialized || consoleInputField == null) return; // Защита от повторной инициализации

        // Очищаем старые обработчики (на всякий случай)
        consoleInputField.onValueChanged.RemoveAllListeners();
        consoleInputField.onSubmit.RemoveAllListeners();

        protectedConsoleContent = commandPrompt;

        isSettingTextProgrammatically = true;
        consoleInputField.text = protectedConsoleContent;
        SetCaretToEnd();

        // Подписываемся на события только один раз
        consoleInputField.onValueChanged.AddListener(OnInputValueChanged);
        consoleInputField.onSubmit.AddListener(OnInputSubmit);

        isSettingTextProgrammatically = false;
        isInitialized = true; // Отмечаем, что инициализация завершена
    }

    public void ResetConsole()
    {
        if (consoleInputField == null) return;

        // Метод для полного сброса консоли (если нужно)
        isSettingTextProgrammatically = true;
        protectedConsoleContent = commandPrompt;
        consoleInputField.text = protectedConsoleContent;
        SetCaretToEnd();
        isSettingTextProgrammatically = false;
    }

    private void OnInputValueChanged(string newText)
    {
        if (isSettingTextProgrammatically || consoleInputField == null) return;

        //Debug.Log("OnInputValueChanged");
        // Запрещаем удаление пути
        if (newText.Length < protectedConsoleContent.Length || !newText.StartsWith(protectedConsoleContent))
        {
            isSettingTextProgrammatically = true;
            consoleInputField.text = protectedConsoleContent;
            SetCaretToEnd();
            isSettingTextProgrammatically = false;
            return;
        }
    }

    private void OnInputSubmit(string newText)
    {
        if (isSettingTextProgrammatically || consoleInputField == null) return;

        //Debug.Log(newText);
        //Debug.Log(protectedConsoleContent);
        // Извлекаем команду (убираем путь)

        string command = newText.Substring(protectedConsoleContent.Length).Trim();

        //Debug.Log(command);

        if (string.IsNullOrEmpty(command))
        {
            // Если команда пустая, просто добавляем новую строку с приглашением
            AddNewPrompt();
            return;
        }

        // Обрабатываем команду и получаем результат
        string commandResult = ProcessCommand(command);

        if (string.IsNullOrEmpty(commandResult)) { return; }

        // Обновляем консоль: текущий текст + результат + новое приглашение
        isSettingTextProgrammatically = true;

        protectedConsoleContent += command + commandResult + "\n\n" + commandPrompt;
        consoleInputField.text = protectedConsoleContent;
        SetCaretToEnd();
        consoleInputField.ActivateInputField();

        isSettingTextProgrammatically = false;
    }

    private string ProcessCommand(string command)
    {
        //Debug.Log("ProcessCommand");
        if (command == "ipconfig")
        {
            return GetIpConfigResult();
        }
        else if (command == "getmac")
        {
            return GetMacResult();
        }
        else if (command.StartsWith("ping"))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1)
            {
                string ip = parts[1];
                string pattern = @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$";

                if (Regex.IsMatch(ip, pattern))
                {
                    return GetPingSuccessResult(ip);
                }
                else
                {
                    return GetInvalidIpResult();
                }
            }
            return GetInvalidIpResult();
        }
        else if (command == "clear")
        {
            ClearConsole();
            return "";
        }
        else
        {
            return $"\n'{command}' is not recognized as an internal or external command.";
        }
    }

    private void AddNewPrompt()
    {
        if (consoleInputField == null) return;

        //Debug.Log("AddNewPrompt");
        isSettingTextProgrammatically = true;

        protectedConsoleContent += "\n" + commandPrompt;
        consoleInputField.text = protectedConsoleContent;
        SetCaretToEnd();
        consoleInputField.ActivateInputField();

        isSettingTextProgrammatically = false;
    }

    private void ClearConsole()
    {

        if (consoleInputField == null) return;

        //Debug.Log("ClearConsole");
        isSettingTextProgrammatically = true;

        consoleInputField.text = commandPrompt;
        protectedConsoleContent = commandPrompt;
        SetCaretToEnd();
        consoleInputField.ActivateInputField();

        isSettingTextProgrammatically = false;
    }

    private void SetCaretToEnd()
    {
        if (consoleInputField == null) return;

        //Debug.Log("SetCaretToEnd");
        consoleInputField.caretPosition = consoleInputField.text.Length;
        consoleInputField.selectionAnchorPosition = consoleInputField.text.Length;
        consoleInputField.selectionFocusPosition = consoleInputField.text.Length;
    }

    private string GetIpConfigResult()
    {
        //Debug.Log("GetIpConfigResult");
        if (mouseDetector == null || mouseDetector.target == null)
            return "\nError: No target device selected.";

        IpConfig ipconfig = mouseDetector.target.GetComponent<IpConfig>();
        if (ipconfig == null)
            return "\nError: No IP configuration available.";

        StringBuilder sb = new StringBuilder();
        sb.Append("\n\nEthernet adapter Ethernet:");
        sb.Append("\n\n\tConnection-specific DNS Suffix  :");
        sb.Append("\n\tLink-local IPv6 Address . . . . : fe80::e5d8:c103:af14:c37b%11");
        sb.Append($"\n\tIPv4 Address. . . . . . . . . . . : {ipconfig.ip}");
        sb.Append($"\n\tSubnet Mask . . . . . . . . . . . : {ipconfig.subnetMask}");
        sb.Append($"\n\tDefault Gateway . . . . . . . . . : {ipconfig.gateway}");

        return sb.ToString();
    }

    private string GetMacResult()
    {
        //Debug.Log("GetMacResult");
        if (mouseDetector == null || mouseDetector.target == null)
            return "\nError: No target device selected.";

        MAC mac = mouseDetector.target.GetComponent<MAC>();
        if (mac == null)
            return "\nError: No MAC address available.";

        StringBuilder sb = new StringBuilder();
        sb.Append("\n\n");
        sb.Append("Physical Address\tTransport Name");
        sb.Append("\n================\t======================================");
        sb.Append($"\n{mac.MACAddres}\t\tMedia disconnected");

        return sb.ToString();
    }

    private string GetPingSuccessResult(string ip)
    {
        //Debug.Log("GetPingSuccessResult");
        StringBuilder sb = new StringBuilder();
        sb.Append($"\n\nPinging {ip} with 32 bytes of data:");
        sb.Append($"\nReply from {ip}: bytes=32 time=1ms TTL=64");
        sb.Append($"\nReply from {ip}: bytes=32 time=1ms TTL=64");
        sb.Append($"\nReply from {ip}: bytes=32 time=1ms TTL=64");
        sb.Append($"\nReply from {ip}: bytes=32 time=1ms TTL=64");

        return sb.ToString();
    }

    private string GetInvalidIpResult()
    {
        //Debug.Log("GetInvalidIpResult");
        return "\n\nPing request could not find host. Please check the name and try again.";
    }
}