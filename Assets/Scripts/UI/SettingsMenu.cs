using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Previous Scene")]

    [SerializeField] private string MainMenu = "MainMenu";



    [Header("Sub Panels")]

    [SerializeField] private GameObject graphicsPanel;

    [SerializeField] private GameObject audioPanel;

    [SerializeField] private GameObject controlsPanel;

    [SerializeField] private GameObject gameplayPanel;

    [SerializeField] private GameObject languagePanel;

    // Panelleri bir dizi içinde toplamak kontrolü kolaylaþtýrýr
    private GameObject[] AllPanels => new GameObject[]
    {
        graphicsPanel, audioPanel, controlsPanel, gameplayPanel, languagePanel
    };

    public void BackButtonPressed()
    {
        if (AreAllPanelsActive())
        {
            // Eðer hepsi zaten açýksa ana menüye dön
            SceneManager.LoadScene(MainMenu);
        }
        else
        {
            // En az biri kapalýysa hepsini aç
            OpenAllPanels();
        }
    }

    private bool AreAllPanelsActive()
    {
        foreach (var panel in AllPanels)
        {
            // Panel atanmýþsa ve sahne üzerinde kapalýysa (activeSelf) false döndür
            if (panel != null && !panel.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    public void OpenAllPanels()
    {
        foreach (var panel in AllPanels)
        {
            panel?.SetActive(true);
        }
    }

    private void CloseAllPanels()
    {
        foreach (var panel in AllPanels)
        {
            panel?.SetActive(false);
        }
    }


    [Header("Language Panel")]

    [SerializeField] private GameObject languageOptionsPanel;



    public void BackToMainMenu()

    {

        SceneManager.LoadScene(MainMenu);

    }
    public void OpenGraphics()
    {
        CloseAllPanels();
        graphicsPanel?.SetActive(true);
    }

    public void OpenAudio()
    {
        CloseAllPanels();
        audioPanel.SetActive(true);
    }


    public void OpenControls()
    {
        CloseAllPanels();
        controlsPanel?.SetActive(true);
    }

    public void OpenGameplay()
    {
        CloseAllPanels();
        gameplayPanel?.SetActive(true);
    }

    public void OpenLanguage()
    {
        CloseAllPanels();
        languageOptionsPanel?.SetActive(true);
    }

}