using UnityEngine;

public class BuildSpot : MonoBehaviour
{
    public bool isOccupied = false;      // Bu slota kule dikildi mi?
    public Transform buildPoint;         // Kule tam nereye konacak?

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
        // Eðer zaten kule varsa, bir þey yapma
        if (isOccupied) return;

        if (BuildMenu.Instance == null)
        {
            Debug.LogError("BuildMenu.Instance is NULL!");   // <<< KONTROL 2
            return;
        }


        // Menü aç, bu spotu gönder
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


    // BuildMenu burayý çaðýracak
    public void BuildTower(GameObject towerPrefab)
    {
        if (towerPrefab == null) return;

        Instantiate(towerPrefab, buildPoint.position, Quaternion.identity);
        isOccupied = true;
    }
}
