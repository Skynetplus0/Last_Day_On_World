using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zombie üzerinde görünen World Space health bar - KIRMIZI TEMA
/// Enemy prefab'ına eklenmeli
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    [Header("Auto Setup")]
    [Tooltip("Otomatik olarak health bar oluştur")]
    public bool autoCreateHealthBar = true;
    
    [Header("Health Bar UI")]
    public Image healthFillImage;
    public Image backgroundImage;
    public Canvas healthBarCanvas;
    
    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 2.5f, 0);
    public Vector2 barSize = new Vector2(1.5f, 0.2f);
    public bool billboardToCamera = true;
    
    [Header("Colors")]
    public Color fullHealthColor = new Color(0.2f, 0.8f, 0.2f);   // Yeşil
    public Color midHealthColor = new Color(0.9f, 0.6f, 0.1f);    // Turuncu
    public Color lowHealthColor = new Color(0.9f, 0.2f, 0.2f);    // Kırmızı
    public Color backgroundColor = new Color(0.15f, 0.15f, 0.2f, 0.9f);
    
    private Enemy enemy;
    private Camera mainCamera;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        mainCamera = Camera.main;
        
        if (autoCreateHealthBar && healthBarCanvas == null)
        {
            CreateHealthBar();
        }
    }

    private void Update()
    {
        if (enemy == null || healthFillImage == null) return;
        
        // Billboard - kameraya bak
        if (billboardToCamera && healthBarCanvas != null && mainCamera != null)
        {
            healthBarCanvas.transform.position = transform.position + offset;
            healthBarCanvas.transform.LookAt(
                healthBarCanvas.transform.position + mainCamera.transform.forward
            );
        }
        
        // Health bar güncelle
        float healthPercent = enemy.GetHealthPercent();
        healthFillImage.fillAmount = healthPercent;
        
        // Renk değişimi
        if (healthPercent > 0.6f)
            healthFillImage.color = fullHealthColor;
        else if (healthPercent > 0.3f)
            healthFillImage.color = midHealthColor;
        else
            healthFillImage.color = lowHealthColor;
    }

    void CreateHealthBar()
    {
        // Canvas oluştur
        GameObject canvasObj = new GameObject("HealthBarCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = offset;
        
        healthBarCanvas = canvasObj.AddComponent<Canvas>();
        healthBarCanvas.renderMode = RenderMode.WorldSpace;
        
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = barSize;
        canvasRect.localScale = Vector3.one * 0.01f; // World space için küçült
        
        // Background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        bgRect.localPosition = Vector3.zero;
        
        backgroundImage = bgObj.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        
        // Fill
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(canvasObj.transform, false);
        
        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0.1f);
        fillRect.anchorMax = new Vector2(1, 0.9f);
        fillRect.sizeDelta = Vector2.zero;
        fillRect.offsetMin = new Vector2(2, 0);
        fillRect.offsetMax = new Vector2(-2, 0);
        fillRect.localPosition = Vector3.zero;
        
        healthFillImage = fillObj.AddComponent<Image>();
        healthFillImage.color = fullHealthColor;
        healthFillImage.type = Image.Type.Filled;
        healthFillImage.fillMethod = Image.FillMethod.Horizontal;
        healthFillImage.fillOrigin = 0;
        healthFillImage.fillAmount = 1f;
    }

    /// <summary>
    /// Health bar'ı göster/gizle
    /// </summary>
    public void SetVisible(bool visible)
    {
        if (healthBarCanvas != null)
            healthBarCanvas.gameObject.SetActive(visible);
    }
}
