using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Названия сцен (должны быть в Build Settings)")]
    public string levelSceneName = "Level5k";
    public string citySceneName = "City"; 

    public void LoadLevel() => SceneManager.LoadScene(levelSceneName);
    public void ReturnToCity() => SceneManager.LoadScene(citySceneName);
}