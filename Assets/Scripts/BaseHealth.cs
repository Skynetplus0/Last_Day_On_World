using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Yol sonundaki evlerin sağlığını yönetir
/// Her ev için ayrı can barı ve toplam base health slider
/// </summary>
public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance;
    
    [Header("Base Settings")]
    [Tooltip("Toplam başlangıç canı")]
    public int maxHealth = 100;
    public int currentHealth;
    
    [Header("UI References")]
    [Tooltip("Ana sağlık slider'ı (sol üstte)")]
    public Slider baseHealthSlider;
    public Image sliderFillImage;
    public TextMeshProUGUI healthText;
    
    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;
    
    [Header("Houses")]
    [Tooltip("Sahnedeki tüm ev objeleri")]
    public List<HouseHealth> houses = new List<HouseHealth>();
    
    // Events
    public System.Action<int> OnHealthChanged;
    public System.Action OnGameOver;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        // Auto-find houses if not assigned
        if (houses.Count == 0)
        {
            houses.AddRange(FindObjectsByType<HouseHealth>(FindObjectsSortMode.None));
        }
    }

    /// <summary>
    /// Zombie eve ulaştığında hasar ver
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        OnHealthChanged?.Invoke(currentHealth);
        UpdateUI();
        
        Debug.Log($"[BaseHealth] Hasar alındı! Kalan: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    /// <summary>
    /// UI elementlerini güncelle
    /// </summary>
    void UpdateUI()
    {
        if (baseHealthSlider != null)
        {
            baseHealthSlider.maxValue = maxHealth;
            baseHealthSlider.value = currentHealth;
        }
        
        if (sliderFillImage != null)
        {
            float percent = (float)currentHealth / maxHealth;
            if (percent > 0.6f)
                sliderFillImage.color = Color.green;
            else if (percent > 0.3f)
                sliderFillImage.color = new Color(1f, 0.5f, 0f); // Orange
            else
                sliderFillImage.color = Color.red;
        }
        
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    /// <summary>
    /// Oyun bitti ekranını göster
    /// </summary>
    void TriggerGameOver()
    {
        Debug.Log("[BaseHealth] GAME OVER!");
        
        OnGameOver?.Invoke();
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (gameOverText != null)
                gameOverText.text = "YOU LOST!";
            
            if (finalScoreText != null)
            {
                int score = ScoreManager.Instance != null ? ScoreManager.Instance.currentScore : 0;
                finalScoreText.text = $"Score: {score}";
            }
        }
        
        // Oyunu durdur
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Oyunu yeniden başlat
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}
