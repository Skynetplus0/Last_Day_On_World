using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Wave ilerleme UI sistemi
/// Slider bar ile mevcut wave'i gösterir
/// 5 wave sonunda Level 3'e geçiş
/// </summary>
public class WaveProgressUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider waveProgressSlider;
    public Image sliderFillImage;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI currentWaveText;
    
    [Header("Level Transition")]
    [Tooltip("Geçiş için gereken wave sayısı")]
    public int wavesToComplete = 5;
    
    [Tooltip("Sonraki level sahnesi adı")]
    public string nextLevelSceneName = "level 3";
    
    [Header("Victory Screen")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;
    public TextMeshProUGUI victoryScoreText;
    
    [Header("References")]
    public Level2EnemySpawner spawner;
    
    private int completedWaves = 0;

    private void Start()
    {
        // Auto-find spawner
        if (spawner == null)
        {
            spawner = FindFirstObjectByType<Level2EnemySpawner>();
        }
        
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
        
        UpdateUI();
    }

    /// <summary>
    /// Wave tamamlandığında çağrılır
    /// </summary>
    public void OnWaveCompleted()
    {
        completedWaves++;
        
        // Puan ekle
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddWavePoints();
        }
        
        UpdateUI();
        
        Debug.Log($"[WaveProgress] Wave {completedWaves}/{wavesToComplete} tamamlandı!");
        
        // Tüm wave'ler tamamlandı mı?
        if (completedWaves >= wavesToComplete)
        {
            ShowVictory();
        }
    }

    void UpdateUI()
    {
        if (waveProgressSlider != null)
        {
            waveProgressSlider.maxValue = wavesToComplete;
            waveProgressSlider.value = completedWaves;
        }
        
        if (sliderFillImage != null)
        {
            float percent = (float)completedWaves / wavesToComplete;
            sliderFillImage.color = Color.Lerp(Color.yellow, Color.green, percent);
        }
        
        if (waveText != null)
        {
            waveText.text = $"{completedWaves} / {wavesToComplete}";
        }
        
        if (currentWaveText != null)
        {
            currentWaveText.text = $"Wave {completedWaves + 1}";
        }
    }

    void ShowVictory()
    {
        Debug.Log("[WaveProgress] VICTORY! Tüm wave'ler tamamlandı!");
        
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
            if (victoryText != null)
                victoryText.text = "LEVEL COMPLETE!";
            
            if (victoryScoreText != null && ScoreManager.Instance != null)
                victoryScoreText.text = $"Score: {ScoreManager.Instance.currentScore}";
        }
        
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Sonraki level'e geç
    /// </summary>
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelSceneName);
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Bu level'i yeniden başlat
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetCompletedWaves()
    {
        return completedWaves;
    }

    public float GetProgress()
    {
        return (float)completedWaves / wavesToComplete;
    }
}
