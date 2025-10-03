using UnityEngine;
using System.Collections.Generic;

public class GlobalEvents : MonoBehaviour
{
    private static GlobalEvents _instance;

    public static GlobalEvents Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GlobalEvents>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GlobalEvents");
                    _instance = go.AddComponent<GlobalEvents>();
                }
            }
            return _instance;
        }
    }

    // Словарь для отслеживания вызванных функций
    private Dictionary<string, bool> executedFunctions = new Dictionary<string, bool>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Метод для отметки выполнения функции
    public void MarkFunctionExecuted(string functionName)
    {
        if (!executedFunctions.ContainsKey(functionName))
        {
            executedFunctions.Add(functionName, true);
        }
        else
        {
            executedFunctions[functionName] = true;
        }
        Debug.Log($"Функция '{functionName}' отмечена как выполненная");
    }

    // Проверка была ли функция выполнена
    public bool WasFunctionExecuted(string functionName)
    {
        return executedFunctions.ContainsKey(functionName) && executedFunctions[functionName];
    }

    // Сброс всех отметок (для перезапуска уровня)
    public void ResetAllExecutions()
    {
        executedFunctions.Clear();
        Debug.Log("Все отметки о выполнении функций сброшены");
    }

    // Методы событий с автоматическим отслеживанием
    public void RouterChangeAuthData()
    {
        Debug.Log("Установлены новые данные для входа в Router");
        MarkFunctionExecuted("RouterChangeAuthData");
    }

    public void RouterChangeWifiData5g()
    {
        Debug.Log("Установлены новые данные для подключения к сети в Router 5g");
        MarkFunctionExecuted("RouterChangeWifiData5g");
    }

    public void RouterChangeWifiData2g()
    {
        Debug.Log("Установлены новые данные для подключения к сети в Router 2g");
        MarkFunctionExecuted("RouterChangeWifiData2g");
    }

    public void EnterToRouter()
    {
        Debug.Log("Вошли в Router");
        MarkFunctionExecuted("EnterToRouter");
    }

    public void ComputerConnectedRouter()
    {
        Debug.Log("Компьютер подключен к Router");
        MarkFunctionExecuted("ComputerConnectedRouter");
    }
}