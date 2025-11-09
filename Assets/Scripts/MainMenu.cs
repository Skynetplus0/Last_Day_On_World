using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string gameSceneName = "SampleScene"; // Oyun sahnenin adý

    public void PlayGame()
    {
        // Ýstersen fade/transition ekleyebilirsin
        SceneManager.LoadScene(gameSceneName);
    }

    public void OpenOptions()
    {
        // Basit yöntem: Options panelini aç/kapat
        // Options panelini Inspector’dan baðlayacaksýn
        if (optionsPanel) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (optionsPanel) optionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        // Editör’de çalýþmaz, build’de çalýþýr
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [Header("Optional")]
    [SerializeField] GameObject optionsPanel; // Inspector’dan atayacaksýn
}