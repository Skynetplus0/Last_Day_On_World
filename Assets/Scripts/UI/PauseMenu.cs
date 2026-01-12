using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public static bool IsPaused = false;
    
    [Header("UI Panels")]
    public GameObject pauseMenuPanel;
    public GameObject optionsPanel;
    
    [Header("Pause Button")]
    public Button pauseButton;
    
    [Header("Menu Buttons")]
    public Button resumeButton;
    public Button optionsButton;
    public Button exitButton;
    
    [Header("Options")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button backButton;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OpenOptions);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
        if (backButton != null)
            backButton.onClick.AddListener(CloseOptions);
            
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 0.3f;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        
        IsPaused = false;
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel != null && optionsPanel.activeSelf)
                CloseOptions();
            else
                TogglePause();
        }
    }
    
    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }
    
    public void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }
    
    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
    
    public void OpenOptions()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }
    
    public void CloseOptions()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.SetMusicVolume(value);
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.Instance != null && SoundManager.Instance.sfxSource != null)
            SoundManager.Instance.sfxSource.volume = value;
    }
    
    public void ExitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
    
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
