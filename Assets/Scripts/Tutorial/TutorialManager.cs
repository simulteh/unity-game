using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public GameObject backgroundPanel;
    public TMP_Text descriptionText;
    public TMP_Text congratulationText;
    public TMP_Text tutorialStepText;

    public GameObject nextButton;
    public GameObject startPlayingButton;
    public GameObject exitButton;

    private int currentStep = 0;
    private bool stepCompleted = false;
    private bool interacted = false;
    private Quaternion lastCameraRotation;

    private Vector3 startPosition;

    private readonly string[] steps = new string[]
    {
        "Поверни камеру с помощью ПКМ.",
        "Передвинься влево (A).",
        "Передвинься вправо (D).",
        "Иди вперёд (W).",
        "Иди назад (S).",
        "Опустись вниз (SPACE).",
        "Поднимись вверх (SHIFT).",
        "Нажми ЛКМ на красный ПК для взаимодействия."
    };

    void Start()
    {
        backgroundPanel.SetActive(true);
        descriptionText.gameObject.SetActive(true);
        congratulationText.gameObject.SetActive(false);
        tutorialStepText.gameObject.SetActive(false);

        nextButton.SetActive(true);
        startPlayingButton.SetActive(false);
        exitButton.SetActive(false);

        startPosition = Camera.main.transform.position;
        lastCameraRotation = Camera.main.transform.rotation;
    }

    void Update()
    {
        if (!tutorialStepText.gameObject.activeSelf || stepCompleted)
            return;

        switch (currentStep)
        {
            case 0: // Камера
                if (Quaternion.Angle(lastCameraRotation, Camera.main.transform.rotation) > 5f)
                    StartCoroutine(CompleteStep());
                break;
            case 1:
                if (Input.GetKey(KeyCode.A))
                    StartCoroutine(CompleteStep());
                break;
            case 2:
                if (Input.GetKey(KeyCode.D))
                    StartCoroutine(CompleteStep());
                break;
            case 3:
                if (Input.GetKey(KeyCode.W))
                    StartCoroutine(CompleteStep());
                break;
            case 4:
                if (Input.GetKey(KeyCode.S))
                    StartCoroutine(CompleteStep());
                break;
            case 5:
                if (Input.GetKey(KeyCode.Space))
                    StartCoroutine(CompleteStep());
                break;
            case 6:
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    StartCoroutine(CompleteStep());
                break;
            case 7:
                if (interacted)
                    StartCoroutine(CompleteStep());
                break;
        }
    }

    public void StartTutorial()
    {
        backgroundPanel.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        tutorialStepText.gameObject.SetActive(true);
        UpdateTutorialText();
    }

    IEnumerator CompleteStep()
    {
        stepCompleted = true;
        tutorialStepText.color = Color.green;

        // Мигаем текстом 3 секунды
        float blinkTime = 3f;
        float timer = 0f;
        bool visible = true;

        while (timer < blinkTime)
        {
            tutorialStepText.alpha = visible ? 1 : 0.3f;
            visible = !visible;
            timer += 0.3f;
            yield return new WaitForSeconds(0.3f);
        }

        tutorialStepText.alpha = 1f;
        tutorialStepText.color = Color.white;

        AdvanceStep();
        stepCompleted = false;
    }

    void AdvanceStep()
    {
        currentStep++;
        interacted = false;

        if (currentStep < steps.Length)
        {
            UpdateTutorialText();
            lastCameraRotation = Camera.main.transform.rotation;
        }
        else
        {
            FinishTutorial();
        }
    }

    void UpdateTutorialText()
    {
        tutorialStepText.text = $"Шаг {currentStep + 1} из {steps.Length}:\n{steps[currentStep]}";
    }

    void FinishTutorial()
    {
        tutorialStepText.gameObject.SetActive(false);
        congratulationText.gameObject.SetActive(true);

        backgroundPanel.SetActive(true);
        nextButton.SetActive(false);
        startPlayingButton.SetActive(true);
        exitButton.SetActive(true);
    }

    public void RegisterInteraction()
    {
        interacted = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1");
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
