using UnityEngine;
using TMPro;

/// <summary>
/// Puan sistemi yöneticisi
/// Zombie öldürme, wave tamamlama gibi aksiyonlarda puan kazanma
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [Header("Score")]
    public int currentScore = 0;
    
    [Header("Points")]
    public int pointsPerKill = 10;
    public int pointsPerWave = 100;
    public int pointsPerBossKill = 50;
    
    [Header("UI")]
    public TextMeshProUGUI scoreText;
    
    // Events
    public System.Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Zombie öldürüldüğünde puan ekle
    /// </summary>
    public void AddKillPoints()
    {
        AddPoints(pointsPerKill);
    }

    /// <summary>
    /// Boss öldürüldüğünde puan ekle
    /// </summary>
    public void AddBossKillPoints()
    {
        AddPoints(pointsPerBossKill);
    }

    /// <summary>
    /// Wave tamamlandığında puan ekle
    /// </summary>
    public void AddWavePoints()
    {
        AddPoints(pointsPerWave);
    }

    /// <summary>
    /// Genel puan ekleme
    /// </summary>
    public void AddPoints(int points)
    {
        currentScore += points;
        OnScoreChanged?.Invoke(currentScore);
        UpdateUI();
        
        Debug.Log($"[Score] +{points} = Toplam: {currentScore}");
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    /// <summary>
    /// Skoru sıfırla
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        UpdateUI();
    }
}
