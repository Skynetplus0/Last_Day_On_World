using UnityEngine;

/// <summary>
/// Zombie/düşman komponenti
/// Can sistemi, ölüm, puan ve para kazanma
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    
    [Header("Enemy Type")]
    public bool isBoss = false;
    
    [Header("Damage to Base")]
    [Tooltip("Eve ulasinca verecegi hasar")]
    public int damageToBase = 1;
    
    [Header("Rewards")]
    [Tooltip("Olduruldugunde kazanilan para")]
    public int coinReward = 20;
    
    private EnemySpawnerNew spawner;
    private bool isDead = false;

    private void Start()
    {
        spawner = FindFirstObjectByType<EnemySpawnerNew>();
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Hasar al
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Mevcut can yüzdesini döndür (health bar için)
    /// </summary>
    public float GetHealthPercent()
    {
        return Mathf.Clamp01(currentHealth / maxHealth);
    }

    /// <summary>
    /// Mevcut canı döndür
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void setSpawner(EnemySpawnerNew s)
    {
        spawner = s;
    }

    /// <summary>
    /// Zombie oldugunde
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        
        // Olum sesi cal (boss ve normal icin ayri)
        if (SoundManager.Instance != null)
        {
            if (isBoss)
                SoundManager.Instance.PlayBossDeath();
            else
                SoundManager.Instance.PlayZombieDeath();
        }
        
        // Para kazandir
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(coinReward);
        }
        
        // Puan kazandir
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKillPoints();
        }
        
        // Spawner'a bildir
        if (spawner != null)
        {
            spawner.onEnemyKilled();
        }
        
        Destroy(gameObject);
    }

    /// <summary>
    /// Eve ulaştığında çağrılır (EnemyMover tarafından)
    /// </summary>
    /// 
    private bool reachedBase=false;

    public void ReachBase()
    {
        if (isDead || reachedBase) return;
        

        reachedBase = true;
        isDead = true;

        // Base'e hasar ver
        if (BaseHealth.Instance != null)
        {
            BaseHealth.Instance.TakeDamage(damageToBase);
        }
        
        // Eski Health sistemi için de (geriye uyumluluk)
        if (Health.Instance != null)
        {
            Health.Instance.TakeDamage(damageToBase);
        }
        
        // Kendini yok et
        Destroy(gameObject);
    }
}
