using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public static BuildMenu Instance;

    public GameObject panel;             // BuildPanel
    public GameObject[] towerPrefabs;    // Seçilebilecek kuleler

    private BuildSpot currentSpot;       // Þu an týklanan slot

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

    // BuildSpot'tan çaðrýlýyor
    public void Open(BuildSpot spot)
    {
        currentSpot = spot;

        if (panel != null)
        {
            panel.SetActive(true);
            // Ýstersen mouse pozisyonuna taþý:
        //    panel.transform.position = Input.mousePosition;
        }
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);

        currentSpot = null;
    }

    // UI Button'lar burayý çaðýracak
    public void BuildTower(int index)
    {
        if (currentSpot == null) return;
        if (towerPrefabs == null || index < 0 || index >= towerPrefabs.Length) return;

        currentSpot.BuildTower(towerPrefabs[index]);
        Close();
    }
}
