using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private EnemySpawnerNew spawner;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void setSpawner(EnemySpawnerNew s)
    {
        spawner = s;
    }

    private void Die()
    {
        // �stersen death animasyonu vs. ekleyebilirsin
        CoinManager.Instance.AddCoins(20); //Sonra değiştirilir 
        spawner?.onEnemyKilled();
        Destroy(gameObject);
        
    }
}
