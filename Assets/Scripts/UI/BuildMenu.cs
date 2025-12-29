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

    // UI Button'lar buray� �a��racak
    public void BuildTower(int index)
    {
        if (currentSpot == null) return;
        if (towerPrefabs == null || index < 0 || index >= towerPrefabs.Length) return;

        int cost = 100; // Sabit kule ücreti

        if (CoinManager.Instance.SpendCoins(cost))
        {
            currentSpot.BuildTower(towerPrefabs[index]);
            Close();
        }
    }
}
