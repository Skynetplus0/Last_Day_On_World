using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BaseHealth : MonoBehaviour
{
    public static BaseHealth Instance;

    [Header("Base Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI References")]
    public Slider baseHealthSlider;
    public Image sliderFillImage;
    public TextMeshProUGUI healthText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;

    [Header("Houses")]
    public List<HouseHealth> houses = new List<HouseHealth>();

    public System.Action<int> OnHealthChanged;
    public System.Action OnGameOver;

    private bool isGameOver = false;   // ✅ EKLENDİ

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (houses.Count == 0)
            houses.AddRange(FindObjectsByType<HouseHealth>(FindObjectsSortMode.None));
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return; // ✅ GameOver sonrası hasarı kes

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        OnHealthChanged?.Invoke(currentHealth);
        UpdateUI();

        // ⚠️ Debug.Log spam donmaya sebep olabilir. Gerekirse kapat:
        // Debug.Log($"[BaseHealth] Hasar alındı! Kalan: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            TriggerGameOver();
    }

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
            if (percent > 0.6f) sliderFillImage.color = Color.green;
            else if (percent > 0.3f) sliderFillImage.color = new Color(1f, 0.5f, 0f);
            else sliderFillImage.color = Color.red;
        }

        if (healthText != null)
            healthText.text = $"{currentHealth} / {maxHealth}";
    }

    void TriggerGameOver()
    {
        if (isGameOver) return;  // ✅ Tek sefer tetikle
        isGameOver = true;

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

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public float GetHealthPercent() => (float)currentHealth / maxHealth;
}