using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public static BuildMenu Instance;

    public GameObject panel;             // BuildPanel
    public GameObject[] towerPrefabs;    // Se�ilebilecek kuleler

    private BuildSpot currentSpot;       // �u an t�klanan slot

    private void Awake()
    {
        // Basit Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    // BuildSpot'tan �a�r�l�yor
    public void Open(BuildSpot spot)
    {
        currentSpot = spot;

        if (panel != null)
        {
            panel.SetActive(true);
            // �stersen mouse pozisyonuna ta��:
        //    panel.transform.position = Input.mousePosition;
        }
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        currentSpot = null;
    }

    // UI Button'lar burayı çağıracak
    public void BuildTower(int index)
    {
        if (currentSpot == null)
        {
            Debug.LogWarning("[BuildMenu] currentSpot is null!");
            return;
        }
        if (towerPrefabs == null || index < 0 || index >= towerPrefabs.Length)
        {
            Debug.LogWarning("[BuildMenu] Invalid tower index or prefabs!");
            return;
        }
        if (towerPrefabs[index] == null)
        {
            Debug.LogWarning("[BuildMenu] Tower prefab at index is null!");
            return;
        }
        int cost = 100;
        if(towerPrefabs[index].GetComponent<TowerBase>() == null)
        {
             cost = 100;
        }
        else
        {
             cost = towerPrefabs[index].GetComponent<TowerBase>().cost;
        }

        // CoinManager null kontrolü
        if (CoinManager.Instance == null)
        {
            Debug.LogWarning("[BuildMenu] CoinManager.Instance is null! Building anyway...");
            currentSpot.BuildTower(towerPrefabs[index]);
            Close();
            return;
        }

        if (CoinManager.Instance.SpendCoins(cost))
        {
            currentSpot.BuildTower(towerPrefabs[index]);
            Close();
        }
    }
}
