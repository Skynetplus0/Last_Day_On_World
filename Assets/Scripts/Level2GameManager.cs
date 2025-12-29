using UnityEngine;

/// <summary>
/// Level 2 için merkezi oyun yöneticisi
/// Tüm sistemleri koordine eder
/// </summary>
public class Level2GameManager : MonoBehaviour
{
    public static Level2GameManager Instance;
    
    [Header("Managers")]
    public BaseHealth baseHealth;
    public ScoreManager scoreManager;
    public WaveProgressUI waveProgressUI;
    public Level2EnemySpawner spawner;
    
    [Header("UI Panels")]
    public GameObject pausePanel;
    
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Auto-find managers
        if (baseHealth == null)
            baseHealth = FindFirstObjectByType<BaseHealth>();
        if (scoreManager == null)
            scoreManager = FindFirstObjectByType<ScoreManager>();
        if (waveProgressUI == null)
            waveProgressUI = FindFirstObjectByType<WaveProgressUI>();
        if (spawner == null)
            spawner = FindFirstObjectByType<Level2EnemySpawner>();
        
        // Game Over event'ine subscribe ol
        if (baseHealth != null)
        {
            baseHealth.OnGameOver += OnGameOver;
        }
        
        Time.timeScale = 1f;
        
        Debug.Log("[GameManager] Level 2 başlatıldı!");
    }

    private void Update()
    {
        // ESC ile pause toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        if (pausePanel != null)
            pausePanel.SetActive(isPaused);
    }

    void OnGameOver()
    {
        Debug.Log("[GameManager] Game Over triggered!");
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

    /// <summary>
    /// Sonraki level'e geç
    /// </summary>
    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("level 3");
    }

    private void OnDestroy()
    {
        if (baseHealth != null)
        {
            baseHealth.OnGameOver -= OnGameOver;
        }
    }
}
