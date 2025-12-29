using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 1000f;          // hareket hızı
    public float edgeSize = 20f;           // ekran kenar hassasiyeti
    public float smoothTime = 0.15f;       // yumuşatma süresi

    private Vector3 velocity = Vector3.zero;

    void CameraMovement()
    {
        Vector3 targetPos = transform.position;

        if (Input.mousePosition.x <= edgeSize)
            targetPos.x -= moveSpeed * Time.deltaTime;

        if (Input.mousePosition.x >= Screen.width - edgeSize)
            targetPos.x += moveSpeed * Time.deltaTime;

        if (Input.mousePosition.y <= edgeSize)
            targetPos.z -= moveSpeed * Time.deltaTime;

        if (Input.mousePosition.y >= Screen.height - edgeSize)
            targetPos.z += moveSpeed * Time.deltaTime;

        //Klavye WASD ile hareket
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        targetPos.x += h * moveSpeed * Time.deltaTime;
        targetPos.z += v * moveSpeed * Time.deltaTime;

        // yumuşak hareket
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
    
    //Camera Focus
    public float normalZoom = 60f;      // normal uzaklık
    public float zoomedIn = 40f;        // çift tıklayınca yaklaşacağı değer
    public float zoomSpeed = 10f;       // zoom animasyon hızı
    public float maxZoom = 60f;
    public float minZoom = 20f;
    public float zoomStep = 10f;

    private float targetZoom;
    private float lastClickTime = 0f; 
    private float doubleClickDelay = 0.25f; // çift tıklama süresi

    void Start()
    {
        targetZoom = normalZoom;
    }

    void Update()
    {
            CameraMovement();
        HandleDoubleClickZoom();
        SmoothZoom();   
    }

void HandleDoubleClickZoom()
{
    if (Input.GetMouseButtonDown(0))
    {
        if (Time.time - lastClickTime < doubleClickDelay)
        {
            // ÇİFT TIKLAMA ALGILANDI
            if (Mathf.Abs(targetZoom - normalZoom) < 0.1f)
            {
                // Zoom In
                targetZoom = zoomedIn;
            }
            else
            {
                // Zoom Out
                targetZoom = normalZoom;
            }
        }

        lastClickTime = Time.time;
    }
}

    void SmoothZoom()
    {
        Camera cam = GetComponent<Camera>();

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }
}