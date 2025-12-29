using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tek bir evin sağlığını yönetir
/// Her evin üzerinde world space health bar olabilir
/// </summary>
public class HouseHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 20;
    public int currentHealth;
    
    [Header("UI (Optional)")]
    public Image healthFillImage;
    public GameObject healthBarCanvas;
    
    [Header("Visual")]
    public GameObject destroyedVisual;
    public GameObject normalVisual;
    
    private bool isDestroyed = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateVisual();
        
        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(true);
    }

    /// <summary>
    /// Eve hasar ver
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        UpdateHealthBar();
        
        if (currentHealth <= 0)
        {
            DestroyHouse();
        }
    }

    void UpdateHealthBar()
    {
        if (healthFillImage != null)
        {
            float percent = (float)currentHealth / maxHealth;
            healthFillImage.fillAmount = percent;
            
            if (percent > 0.6f)
                healthFillImage.color = Color.green;
            else if (percent > 0.3f)
                healthFillImage.color = Color.yellow;
            else
                healthFillImage.color = Color.red;
        }
    }

    void DestroyHouse()
    {
        isDestroyed = true;
        
        if (healthBarCanvas != null)
            healthBarCanvas.SetActive(false);
        
        UpdateVisual();
        
        Debug.Log($"[HouseHealth] {gameObject.name} yıkıldı!");
    }

    void UpdateVisual()
    {
        if (normalVisual != null)
            normalVisual.SetActive(!isDestroyed);
        
        if (destroyedVisual != null)
            destroyedVisual.SetActive(isDestroyed);
    }

    public bool IsDestroyed()
    {
        return isDestroyed;
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }
}
