using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    public bool isOccupied = false;      // Bu slota kule dikildi mi?
    public Transform buildPoint;         // Kule tam nereye konacak?

    //Kulelerin bakaca�� nokta onun i�in noktalar koycam
    public Transform look_Target;

    private void Start()
    {
        // buildPoint yoksa, kendi pozisyonunu kullan
        if (buildPoint == null)
        {
            buildPoint = this.transform;
        }
    }
    /*
    private void OnMouseDown()
    {
        Debug.Log("BuildSpot clicked: " + name);
        // E�er zaten kule varsa, bir �ey yapma
        if (isOccupied) return;

        if (BuildMenu.Instance == null)
        {
            Debug.LogError("BuildMenu.Instance is NULL!");   // <<< KONTROL 2
            return;
        }


        // Men� a�, bu spotu g�nder
        BuildMenu.Instance.Open(this);
    }

    */
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


    // BuildMenu buray� �a��racak
    public float y_Rotation_Offset = 180f;//ayarlanacak yoksa do�ru d�nm�yor

    public void BuildTower(GameObject towerPrefab)
    {
        if (towerPrefab == null) return;
        if (isOccupied) return;

        GameObject tower = Instantiate(towerPrefab, buildPoint.position, Quaternion.identity);
        
        if(look_Target != null) {
        
        Vector3 dir = look_Target.position- tower.transform.position;
         dir.y = 0f;//Y ekseni bozulmas�n

            if(dir.sqrMagnitude > 0.001f)
            {
                Quaternion baseRot = Quaternion.LookRotation(dir);

                // EKSEN FARKI ���N Y OFFSET
                // Silah +X'e bak�yorsa genelde -90 veya +90 derece gerekir
                tower.transform.rotation = baseRot * Quaternion.Euler(0f, y_Rotation_Offset, 0f);
            }


        }
        
        
        
        
        isOccupied = true;

        var rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = false;

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }
}
