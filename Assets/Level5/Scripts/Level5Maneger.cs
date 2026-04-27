using UnityEngine;
using UnityEngine.Events;

public class Level5Manager : MonoBehaviour
{
    public static Level5Manager Instance { get; private set; }

    [Header("Флаги прогресса")]
    public bool isDiagnosed;
    public bool isWanConfigured;
    public bool isNatEnabled;
    public bool isInternetOk;
    public bool isTableSeen;
    public bool isQuizPassed;

    [Header("События для UI")]
    public UnityEvent OnStepCompleted;
    public UnityEvent OnLevelCompleted;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetStep(string step)
    {
        switch (step)
        {
            case "Diagnosed": isDiagnosed = true; break;
            case "WAN_Done": isWanConfigured = true; break;
            case "NAT_On": isNatEnabled = true; break;
            case "Ping_OK": isInternetOk = true; break;
            case "Table_Viewed": isTableSeen = true; break;
            case "Quiz_Passed": isQuizPassed = true; break;
        }
        OnStepCompleted?.Invoke();
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (isWanConfigured && isNatEnabled && isInternetOk && isQuizPassed)
            OnLevelCompleted?.Invoke();
    }

    public void ResetLevel()
    {
        isDiagnosed = false; isWanConfigured = false; isNatEnabled = false;
        isInternetOk = false; isTableSeen = false; isQuizPassed = false;
    }
}