using UnityEngine;
using UnityEngine.SceneManagement;

public class CableMinigameTrigger : MonoBehaviour
{
    public string miniGameSceneName = "CableCrimpMinigame";

    void OnMouseDown()
    {
        SceneManager.LoadScene(miniGameSceneName);
    }
}