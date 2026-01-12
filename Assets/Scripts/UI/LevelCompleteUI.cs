using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelCompleteUI : MonoBehaviour
{
    public static LevelCompleteUI Instance;
    
    [Header("Panel")]
    public GameObject levelCompletePanel;
    
    [Header("Texts")]
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI wavesCompletedText;
    
    [Header("Buttons")]
    public Button continueButton;
    public Button mainMenuButton;
    
    [Header("Next Level")]
    public string nextLevelScene = "level 2";
    public string mainMenuScene = "MainMenu";
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start()
    {
        // Baslangicta gizle
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
        
        // Buton eventleri
        if (continueButton != null)
            continueButton.onClick.AddListener(LoadNextLevel);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);
    }
    
    public void ShowLevelComplete(string levelName, int score, int coins, int wavesCompleted)
    {
        // Oyunu duraklat
        Time.timeScale = 0f;
        
        // Paneli goster
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
        
        // Bilgileri guncelle
        if (levelNameText != null)
            levelNameText.text = levelName + " COMPLETE!";
        
        if (scoreText != null)
            scoreText.text = "SCORE: " + score.ToString();
        
        if (coinsText != null)
            coinsText.text = "COINS: " + coins.ToString();
        
        if (wavesCompletedText != null)
            wavesCompletedText.text = "WAVES: " + wavesCompleted.ToString();
        
        Debug.Log($"[LevelCompleteUI] Level tamamlandi! Score: {score}, Coins: {coins}");
    }
    
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelScene);
    }
    
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
