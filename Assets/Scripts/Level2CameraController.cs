using UnityEngine;

/// <summary>
/// Profesyonel RTS-style kamera kontrolcüsü - Level 2 için optimize edilmiş
/// Özellikler:
/// - WASD / Arrow tuşları ile hareket
/// - Mouse kenar hareketi
/// - Q/E ile yükseklik kontrolü
/// - Mouse scroll ile zoom
/// - Sağ tık + sürükleme ile kamera açısı döndürme
/// - Harita sınırları (kamera dışarı çıkamaz)
/// - Smooth hareket
/// </summary>
public class Level2CameraController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Kamera hareket hızı")]
    public float moveSpeed = 200f;
    
    [Tooltip("Ekran kenarında hareket hassasiyeti (piksel)")]
    public float edgeScrollSize = 25f;
    
    [Tooltip("Kenar hareketi aktif mi?")]
    public bool enableEdgeScroll = true;
    
    [Tooltip("Hareket yumuşatma süresi")]
    public float moveSmoothTime = 0.1f;

    [Header("Yükseklik Kontrolü")]
    [Tooltip("Q/E ile yükseklik değiştirme hızı")]
    public float heightSpeed = 50f;
    
    [Tooltip("Minimum kamera yüksekliği")]
    public float minHeight = 20f;
    
    [Tooltip("Maksimum kamera yüksekliği")]
    public float maxHeight = 120f;

    [Header("Zoom Ayarları")]
    [Tooltip("Zoom hızı (scroll)")]
    public float zoomSpeed = 20f;
    
    [Tooltip("Minimum FOV (yakınlaşma)")]
    public float minFOV = 30f;
    
    [Tooltip("Maksimum FOV (uzaklaşma)")]
    public float maxFOV = 80f;
    
    [Tooltip("Zoom yumuşatma hızı")]
    public float zoomSmoothSpeed = 10f;

    [Header("Döndürme Ayarları")]
    [Tooltip("Sağ tık ile döndürme aktif mi?")]
    public bool enableRotation = true;
    
    [Tooltip("Döndürme hassasiyeti")]
    public float rotationSpeed = 3f;
    
    [Tooltip("Minimum X açısı (yukarı bakış)")]
    public float minPitch = 20f;
    
    [Tooltip("Maksimum X açısı (aşağı bakış)")]
    public float maxPitch = 80f;

    [Header("Harita Sınırları")]
    [Tooltip("Harita sınırlarını kullan")]
    public bool useBounds = true;
    
    [Tooltip("Minimum X pozisyonu")]
    public float minX = -300f;
    
    [Tooltip("Maksimum X pozisyonu")]
    public float maxX = 50f;
    
    [Tooltip("Minimum Z pozisyonu")]
    public float minZ = -150f;
    
    [Tooltip("Maksimum Z pozisyonu")]
    public float maxZ = 50f;

    [Header("Kamera Kilidi")]
    [Tooltip("Kamera hareketini kilitle/aç (Sağ tık ile toggle)")]
    public bool isLocked = false;
    
    [Tooltip("Sağ tık ile kilit toggle")]
    public bool rightClickToToggleLock = false;

    // Private değişkenler
    private Vector3 moveVelocity = Vector3.zero;
    private float targetFOV;
    private float currentYaw;
    private float currentPitch;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        // Başlangıç değerlerini ayarla
        targetFOV = cam != null ? cam.fieldOfView : 60f;
        
        Vector3 euler = transform.eulerAngles;
        currentYaw = euler.y;
        currentPitch = euler.x;
    }

    private void Update()
    {
        // Kilit toggle
        if (rightClickToToggleLock && Input.GetMouseButtonDown(1))
        {
            isLocked = !isLocked;
        }

        if (isLocked) return;

        HandleMovement();
        HandleHeight();
        HandleZoom();
        HandleRotation();
        ApplyBounds();
    }

    /// <summary>
    /// WASD ve kenar hareketi
    /// </summary>
    private void HandleMovement()
    {
        Vector3 targetPos = transform.position;
        
        // Klavye girişi
        float h = Input.GetAxisRaw("Horizontal"); // A/D veya Left/Right
        float v = Input.GetAxisRaw("Vertical");   // W/S veya Up/Down
        
        // Kameranın baktığı yöne göre hareket (Y rotasyonunu kullan)
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        
        Vector3 moveDir = (forward * v + right * h).normalized;
        targetPos += moveDir * moveSpeed * Time.deltaTime;
        
        // Kenar hareketi
        if (enableEdgeScroll)
        {
            Vector3 edgeMove = Vector3.zero;
            
            if (Input.mousePosition.x <= edgeScrollSize)
                edgeMove -= right;
            if (Input.mousePosition.x >= Screen.width - edgeScrollSize)
                edgeMove += right;
            if (Input.mousePosition.y <= edgeScrollSize)
                edgeMove -= forward;
            if (Input.mousePosition.y >= Screen.height - edgeScrollSize)
                edgeMove += forward;
            
            if (edgeMove != Vector3.zero)
                targetPos += edgeMove.normalized * moveSpeed * Time.deltaTime;
        }
        
        // Yumuşak hareket
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref moveVelocity,
            moveSmoothTime
        );
    }

    /// <summary>
    /// Q/E ile yükseklik kontrolü
    /// </summary>
    private void HandleHeight()
    {
        float heightInput = 0f;
        
        if (Input.GetKey(KeyCode.Q))
            heightInput = -1f;
        if (Input.GetKey(KeyCode.E))
            heightInput = 1f;
        
        // Mouse orta tuş + shift ile de yükseklik değiştir
        if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftShift))
        {
            heightInput = Input.GetAxis("Mouse Y") * 2f;
        }
        
        if (heightInput != 0f)
        {
            Vector3 pos = transform.position;
            pos.y += heightInput * heightSpeed * Time.deltaTime;
            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            transform.position = pos;
        }
    }

    /// <summary>
    /// Mouse scroll ile zoom
    /// </summary>
    private void HandleZoom()
    {
        if (cam == null) return;
        
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0f)
        {
            targetFOV -= scroll * zoomSpeed * 10f;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }
        
        // Yumuşak zoom
        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetFOV, Time.deltaTime * zoomSmoothSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSmoothSpeed);
        }
    }

    /// <summary>
    /// Sağ tık + sürükleme ile kamera açısı döndürme
    /// </summary>
    private void HandleRotation()
    {
        if (!enableRotation) return;
        
        // Sağ tık basılıyken (ve lock toggle kapalıysa)
        if (Input.GetMouseButton(1) && !rightClickToToggleLock)
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
            
            currentYaw += mouseX;
            currentPitch -= mouseY;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
        
        // Orta tuş ile de döndürme (shift olmadan)
        if (Input.GetMouseButton(2) && !Input.GetKey(KeyCode.LeftShift))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
            
            currentYaw += mouseX;
            currentPitch -= mouseY;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
            
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
    }

    /// <summary>
    /// Harita sınırlarını uygula
    /// </summary>
    private void ApplyBounds()
    {
        if (!useBounds) return;
        
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
        transform.position = pos;
    }

    /// <summary>
    /// Kamerayı belirli bir pozisyona odakla
    /// </summary>
    public void FocusOn(Vector3 worldPosition)
    {
        Vector3 newPos = worldPosition;
        newPos.y = transform.position.y; // Yüksekliği koru
        transform.position = newPos;
    }

    /// <summary>
    /// Kamerayı belirli bir Transform'a odakla
    /// </summary>
    public void FocusOn(Transform target)
    {
        if (target != null)
            FocusOn(target.position);
    }

    // Editor'da sınırları göster
    private void OnDrawGizmosSelected()
    {
        if (!useBounds) return;
        
        Gizmos.color = Color.yellow;
        
        // Sınır çizgileri
        Vector3 corner1 = new Vector3(minX, 0, minZ);
        Vector3 corner2 = new Vector3(maxX, 0, minZ);
        Vector3 corner3 = new Vector3(maxX, 0, maxZ);
        Vector3 corner4 = new Vector3(minX, 0, maxZ);
        
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);
        
        // Yükseklik sınırları
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3((minX + maxX) / 2, 0, (minZ + maxZ) / 2);
        Gizmos.DrawLine(center + Vector3.up * minHeight, center + Vector3.up * maxHeight);
    }
}
