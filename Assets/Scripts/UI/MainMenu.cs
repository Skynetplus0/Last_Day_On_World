using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "level 1";
    
    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip menuMusic;
    public AudioClip buttonClickSound;
    private AudioSource sfxSource;
    
    [Header("Settings Panel")]
    public GameObject settingsPanel;
    
    [Header("Settings - Audio")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Start()
    {
        // Muzik baslat
        if (musicSource != null && menuMusic != null)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
        
        // SFX source olustur
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        
        // Settings paneli baslangicta gizle
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        
        // Slider degerlerini ayarla
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = 0.5f;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = 1f;
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }
    
    private void PlayButtonSound()
    {
        if (sfxSource != null && buttonClickSound != null)
            sfxSource.PlayOneShot(buttonClickSound);
    }

    public void PlayGame()
    {
        PlayButtonSound();
        // Index ile yuklemek daha guvenli
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        PlayButtonSound();
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    public void CloseSettings()
    {
        PlayButtonSound();
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        if (musicSource != null)
            musicSource.volume = value;
    }
    
    public void OnSFXVolumeChanged(float value)
    {
        if (sfxSource != null)
            sfxSource.volume = value;
            
        // Onizleme sesi cal
        if (buttonClickSound != null)
            sfxSource.PlayOneShot(buttonClickSound, value);
    }

    public void QuitGame()
    {
        PlayButtonSound();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}