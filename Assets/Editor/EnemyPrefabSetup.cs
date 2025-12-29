using UnityEngine;
using UnityEditor;

/// <summary>
/// Enemy prefab'ına gerekli componentleri ekleyen araç
/// </summary>
public class EnemyPrefabSetup : EditorWindow
{
    private GameObject enemyPrefab;

    [MenuItem("Tools/Enemy Prefab Setup")]
    static void Init()
    {
        EnemyPrefabSetup window = GetWindow<EnemyPrefabSetup>("Enemy Setup");
        window.minSize = new Vector2(350, 250);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("🧟 Enemy Prefab Kurulum", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Bu araç Enemy prefab'ına ekler:\n" +
            "• EnemyHealthBar (kırmızı can barı)\n" +
            "• Gerekli componentler\n\n" +
            "Prefab'ı buraya sürükle veya seç:",
            MessageType.Info
        );

        GUILayout.Space(10);

        enemyPrefab = (GameObject)EditorGUILayout.ObjectField("Enemy Prefab:", enemyPrefab, typeof(GameObject), false);

        GUILayout.Space(15);

        // Seçili objeyi kullan butonu
        if (GUILayout.Button("Seçili Objeyi Kullan", GUILayout.Height(25)))
        {
            if (Selection.activeGameObject != null)
            {
                enemyPrefab = Selection.activeGameObject;
            }
        }

        GUILayout.Space(10);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("PREFAB'I GÜNCELLE", GUILayout.Height(40)))
        {
            SetupEnemyPrefab();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(15);

        // Sahnedeki tüm enemy'lere ekle
        if (GUILayout.Button("SAHNEDEKİ TÜM ENEMY'LERE EKLE", GUILayout.Height(35)))
        {
            SetupAllEnemiesInScene();
        }
    }

    void SetupEnemyPrefab()
    {
        if (enemyPrefab == null)
        {
            EditorUtility.DisplayDialog("Hata", "Lütfen bir prefab seçin!", "Tamam");
            return;
        }

        // Prefab mode'a gir
        string assetPath = AssetDatabase.GetAssetPath(enemyPrefab);
        if (string.IsNullOrEmpty(assetPath))
        {
            // Sahne objesi, doğrudan düzenle
            AddComponentsToEnemy(enemyPrefab);
            return;
        }

        // Prefab'ı aç
        GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
        
        AddComponentsToEnemy(prefabRoot);
        
        // Kaydet
        PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
        PrefabUtility.UnloadPrefabContents(prefabRoot);

        EditorUtility.DisplayDialog("Başarılı", 
            "Enemy prefab güncellendi!\n\n" +
            "Eklenen componentler:\n" +
            "• EnemyHealthBar", "Tamam");
    }

    void AddComponentsToEnemy(GameObject obj)
    {
        // Enemy component kontrolü
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy == null)
        {
            enemy = obj.AddComponent<Enemy>();
            Debug.Log("[Setup] Enemy component eklendi.");
        }

        // EnemyHealthBar
        EnemyHealthBar healthBar = obj.GetComponent<EnemyHealthBar>();
        if (healthBar == null)
        {
            healthBar = obj.AddComponent<EnemyHealthBar>();
            healthBar.autoCreateHealthBar = true;
            Debug.Log("[Setup] EnemyHealthBar eklendi.");
        }
    }

    void SetupAllEnemiesInScene()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        int count = 0;

        foreach (Enemy enemy in enemies)
        {
            EnemyHealthBar healthBar = enemy.GetComponent<EnemyHealthBar>();
            if (healthBar == null)
            {
                Undo.AddComponent<EnemyHealthBar>(enemy.gameObject);
                count++;
            }
        }

        EditorUtility.DisplayDialog("Tamamlandı", 
            $"{count} adet Enemy'ye EnemyHealthBar eklendi!", "Tamam");
    }
}
