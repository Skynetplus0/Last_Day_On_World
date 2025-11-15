using UnityEngine;

public class ClickManager : MonoBehaviour
{
    void Update()
    {
        // Sol týk
        if (Input.GetMouseButtonDown(0))
        {
            // Main kameradan bir ray at
            Camera cam = Camera.main;
            if (cam == null)
            {
                Debug.LogError("Main Camera bulunamadý! Kamerana 'MainCamera' tag'ini ver.");
                return;
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Clicked on: " + hit.collider.name);

                // Týklanan objede BuildSpot var mý bak
                BuildSpot spot = hit.collider.GetComponent<BuildSpot>();
                if (spot != null)
                {
                    spot.OnClicked();
                }
            }
        }
    }
}
