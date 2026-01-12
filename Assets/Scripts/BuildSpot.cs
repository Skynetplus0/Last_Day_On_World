using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    public bool isOccupied = false;
    public Transform buildPoint;
    public Transform look_Target;
    public float y_Rotation_Offset = 180f;

    private void Start()
    {
        if (buildPoint == null)
        {
            buildPoint = this.transform;
        }
    }

    public void OnClicked()
    {
        Debug.Log("BuildSpot OnClicked: " + name);

        if (isOccupied) return;

        if (BuildMenu.Instance == null)
        {
            Debug.LogError("BuildMenu.Instance is NULL!");
            return;
        }

        BuildMenu.Instance.Open(this);
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (towerPrefab == null) return;
        if (isOccupied) return;

        // Kuleyi buildPoint pozisyonunda olustur
        // Pozisyonu tam olarak buildPoint'e ayarla
        Vector3 spawnPos = buildPoint.position;
        
        GameObject tower = Instantiate(towerPrefab, spawnPos, Quaternion.identity);
        
        // Kulenin pozisyonunu sifirla (prefab offset'lerini temizle)
        tower.transform.position = spawnPos;

        // Look target varsa ona dogru dondur
        if (look_Target != null)
        {
            Vector3 dir = look_Target.position - tower.transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion baseRot = Quaternion.LookRotation(dir);
                tower.transform.rotation = baseRot * Quaternion.Euler(0f, y_Rotation_Offset, 0f);
            }
        }

        isOccupied = true;

        // BuildSpot'u gizle
        var rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        Debug.Log($"[BuildSpot] Kule insa edildi: {tower.name} pozisyon: {spawnPos}");
    }
}
