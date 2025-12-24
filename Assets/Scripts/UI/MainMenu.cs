using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string settingsMenuScene = "SettingsMenu";

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OpenSettingsScene()
    {
        Debug.Log("Loading: " + settingsMenuScene);
        SceneManager.LoadScene(settingsMenuScene);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}