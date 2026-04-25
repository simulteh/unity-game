using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class LevelsManager : MonoBehaviour
{
    [SerializeField] GameObject infoPanel;
    [SerializeField] TextMeshProUGUI clientName;
    [SerializeField] TextMeshProUGUI clientComment;
    [SerializeField] GameObject buttonStartLevel;

    [SerializeField] GameObject ButtonOrderPrefab;
    [SerializeField] GameObject MainPanel;

    [SerializeField] GameObject SceneManagment;

    LevelsBD levelsBD = new LevelsBD();

    private void Start()
    {
        GameObject buttonOrder = Instantiate(ButtonOrderPrefab, MainPanel.transform);

        buttonOrder.GetComponent<Button>().onClick.AddListener(() =>
        {
            InverseActiveInfoPanel();
            SetActiveLevel(0);
        });

    }

    public void SetActiveLevel(int id)
    {
        Level level = levelsBD.GetLevel(id);

        buttonStartLevel.GetComponent<Button>().onClick.AddListener(() =>
            {
                SceneManagment.GetComponent<SceneManagment>().LoadSceneId(level.scene);
            }
        );

        clientName.text = level.clientName;
        clientComment.text = level.clientComment;
    }

    public void CloseInfoPanel() { infoPanel.SetActive(false); }

    public void OpenInfoPanel() { infoPanel.SetActive(true); }

    public void InverseActiveInfoPanel() { infoPanel.SetActive(!infoPanel.activeSelf); }

}

class LevelsBD
{
    List<Level> levels = new List<Level>()
    {
        new Level()
        {
            scene = 2,
            clientName = "Ольга",
            clientComment = "Мне необходимо настроить домашний wi-fi",

        },
    };

    public Level GetLevel(int id)
    {
        return levels[id];
    }
}

class Level
{
    public int scene { get; set; }
    public string clientName { get; set; }
    public string clientComment { get; set; }
}
