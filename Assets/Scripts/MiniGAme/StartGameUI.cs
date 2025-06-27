using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject successPanel;
    public GameObject failPanel;

    [Header("Game Elements")]
    public Text timerText;
    public float timeLeft = 60f;
    private bool timerRunning = false;
    private bool gameEnded = false;

    [Header("Ссылка на CrimpValidator")]
    public CrimpValidator validator;

    void Start()
    {
        startPanel.SetActive(true);
        successPanel.SetActive(false);
        failPanel.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    public void StartMinigame()
    {
        startPanel.SetActive(false);
        timerText.gameObject.SetActive(true);
        timerRunning = true;
    }

    void Update()
    {
        if (!timerRunning || gameEnded)
            return;

        timeLeft -= Time.deltaTime;
        timerText.text = $"Осталось времени: {Mathf.CeilToInt(timeLeft)} сек";

        if (timeLeft <= 0)
        {
            timerRunning = false;
            EndGame(false);
        }
    }

    public void CheckVictory()
    {
        if (gameEnded) return;

        if (validator.AllSlotsCorrect())
        {
            EndGame(true);
        }
        else
        {
            Debug.Log(" Не все провода на местах.");
        }
    }

    void EndGame(bool success)
    {
        gameEnded = true;
        timerRunning = false;

        if (success)
        {
            successPanel.SetActive(true);
            Invoke("BackToMainScene", 3f); // Через 3 секунды вернёт в Scene1
        }
        else
        {
            failPanel.SetActive(true);
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void BackToMainScene()
    {
        SceneManager.LoadScene("Scene1");
    }
}
