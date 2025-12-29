using UnityEngine;
using UnityEditor;

/// <summary>
/// Level 2 haritasini yeniden yapilandirir
/// - Tek buyuk ground
/// - Tum yollar tek base'e birlesir
/// - Buyuk ana ev
/// </summary>
public class Level2MapRestructure : EditorWindow
{
    // Ground ayarlari
    private Vector2 groundSize = new Vector2(500, 800);
    private Material groundMaterial;
    
    // Base ayarlari
    private Vector3 basePosition = new Vector3(0, 0, 350);
    private GameObject housePrefab;
    
    // Kamera ayarlari
    private float cameraSpeed = 300f;
    
    private bool deleteOldGround = true;
    private bool deleteOldProps = true;

    [MenuItem("Tools/Level 2 Map Restructure (Harita Yeniden Yapi)")]
    static void Init()
    {
        Level2MapRestructure window = GetWindow<Level2MapRestructure>("Map Restructure");
        window.minSize = new Vector2(420, 500);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Level 2 Harita Yapilandirma", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Bu arac:\n" +
            "1. Eski ground objelerini siler\n" +
            "2. Tek buyuk ground olusturur\n" +
            "3. Yollari base'e yonlendirir\n" +
            "4. Buyuk base evi yerlestirir\n" +
            "5. Kamerayi hizlandirir",
            MessageType.Info
        );

        GUILayout.Space(15);

        // Ground ayarlari
        EditorGUILayout.LabelField("Ground Ayarlari", EditorStyles.boldLabel);
        groundSize = EditorGUILayout.Vector2Field("Boyut (X, Z):", groundSize);
        groundMaterial = (Material)EditorGUILayout.ObjectField("Material:", groundMaterial, typeof(Material), false);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GrassMat Yukle"))
        {
            LoadMaterial("GrassMat");
        }
        if (GUILayout.Button("GrassMatImage"))
        {
            LoadMaterial("GrassMatImage");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Base ayarlari
        EditorGUILayout.LabelField("Base/Ev Ayarlari", EditorStyles.boldLabel);
        basePosition = EditorGUILayout.Vector3Field("Base Pozisyon:", basePosition);
        housePrefab = (GameObject)EditorGUILayout.ObjectField("Ev Prefab:", housePrefab, typeof(GameObject), false);
        
        if (GUILayout.Button("House Prefab Bul"))
        {
            FindHousePrefab();
        }

        GUILayout.Space(10);

        // Kamera ayarlari
        EditorGUILayout.LabelField("Kamera Ayarlari", EditorStyles.boldLabel);
        cameraSpeed = EditorGUILayout.Slider("Hiz:", cameraSpeed, 100f, 500f);

        GUILayout.Space(10);

        // Silme secenekleri
        deleteOldGround = EditorGUILayout.Toggle("Eski Ground'lari Sil", deleteOldGround);
        deleteOldProps = EditorGUILayout.Toggle("Eski Props'lari Sil", deleteOldProps);

        GUILayout.Space(20);

        // Ana butonlar
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("TUM HARITAYI YENIDEN YAPILANDIR", GUILayout.Height(50)))
        {
            RestructureMap();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        // Tek tek butonlar
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Sadece Ground", GUILayout.Height(30)))
        {
            CreateSingleGround();
        }
        if (GUILayout.Button("Sadece Base", GUILayout.Height(30)))
        {
            CreateBaseHouse();
        }
        if (GUILayout.Button("Kamera Hizla", GUILayout.Height(30)))
        {
            SpeedUpCamera();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("ESKI GROUND'LARI SIL", GUILayout.Height(30)))
        {
            DeleteOldGrounds();
        }
        GUI.backgroundColor = Color.white;
    }

    void LoadMaterial(string name)
    {
        string[] guids = AssetDatabase.FindAssets($"{name} t:Material");
        if (guids.Length > 0)
        {
            groundMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[0]));
            Debug.Log($"[MapRestructure] Material yuklendi: {name}");
        }
    }

    void FindHousePrefab()
    {
        string[] guids = AssetDatabase.FindAssets("House t:Prefab", new[] { "Assets/ALP_Assets" });
        if (guids.Length > 0)
        {
            housePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
            Debug.Log("[MapRestructure] House prefab bulundu!");
        }
    }

    void RestructureMap()
    {
        if (deleteOldGround) DeleteOldGrounds();
        if (deleteOldProps) DeleteOldProps();
        
        CreateSingleGround();
        CreateBaseHouse();
        SpeedUpCamera();
        UpdatePathNodes();
        
        EditorUtility.DisplayDialog("Tamamlandi", 
            "Harita yeniden yapilandirildi!\n\n" +
            "- Tek buyuk ground olusturuldu\n" +
            "- Base evi yerlestirildi\n" +
            "- Kamera hizlandirildi\n" +
            "- PathNode'lar guncellendi\n\n" +
            "Ctrl+S ile kaydedin!", "Tamam");
    }

    void DeleteOldGrounds()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        System.Collections.Generic.List<GameObject> toDelete = new System.Collections.Generic.List<GameObject>();
        
        // Once listeye topla
        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;
            if (obj.name.Contains("Ground") && obj.GetComponent<Renderer>() != null)
            {
                toDelete.Add(obj);
            }
        }
        
        // Sonra sil
        int count = 0;
        foreach (GameObject obj in toDelete)
        {
            if (obj != null)
            {
                Undo.DestroyObjectImmediate(obj);
                count++;
            }
        }
        
        Debug.Log($"[MapRestructure] {count} eski ground silindi!");
    }

    void DeleteOldProps()
    {
        string[] containers = { "Props_Trees", "Props_Rocks", "Props_Houses", "Props_Lights", "Props_Grass" };
        foreach (string name in containers)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }
        Debug.Log("[MapRestructure] Prop'lar silindi!");
    }

    void CreateSingleGround()
    {
        // Container
        GameObject container = GameObject.Find("=== GROUND ===");
        if (container == null)
        {
            container = new GameObject("=== GROUND ===");
            Undo.RegisterCreatedObjectUndo(container, "Create Ground Container");
        }

        // Buyuk ground plane
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "MainGround";
        ground.transform.parent = container.transform;
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(groundSize.x / 10f, 1, groundSize.y / 10f);
        
        if (groundMaterial != null)
        {
            Renderer r = ground.GetComponent<Renderer>();
            r.sharedMaterial = groundMaterial;
        }
        
        Undo.RegisterCreatedObjectUndo(ground, "Create Main Ground");
        Debug.Log($"[MapRestructure] Ground olusturuldu: {groundSize.x}x{groundSize.y}");
    }

    void CreateBaseHouse()
    {
        // Container
        GameObject container = GameObject.Find("=== BASE ===");
        if (container == null)
        {
            container = new GameObject("=== BASE ===");
            Undo.RegisterCreatedObjectUndo(container, "Create Base Container");
        }

        // Buyuk base evi
        GameObject baseHouse;
        
        if (housePrefab != null)
        {
            baseHouse = (GameObject)PrefabUtility.InstantiatePrefab(housePrefab, container.transform);
            baseHouse.name = "MainBase";
            baseHouse.transform.position = basePosition;
            baseHouse.transform.localScale = Vector3.one * 3f; // 3x buyuk
        }
        else
        {
            // Prefab yoksa basit kup
            baseHouse = GameObject.CreatePrimitive(PrimitiveType.Cube);
            baseHouse.name = "MainBase";
            baseHouse.transform.parent = container.transform;
            baseHouse.transform.position = basePosition;
            baseHouse.transform.localScale = new Vector3(20, 15, 20);
            
            // Renklendir
            Renderer r = baseHouse.GetComponent<Renderer>();
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.6f, 0.3f, 0.1f); // Kahverengi
            r.sharedMaterial = mat;
        }
        
        // HouseHealth ekle
        if (baseHouse.GetComponent<HouseHealth>() == null)
        {
            HouseHealth hh = baseHouse.AddComponent<HouseHealth>();
            hh.maxHealth = 500; // Buyuk ev, cok can
        }
        
        Undo.RegisterCreatedObjectUndo(baseHouse, "Create Base House");
        Debug.Log($"[MapRestructure] Base evi olusturuldu: {basePosition}");
    }

    void SpeedUpCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Level2CameraController controller = mainCam.GetComponent<Level2CameraController>();
        if (controller == null)
        {
            controller = Undo.AddComponent<Level2CameraController>(mainCam.gameObject);
        }

        Undo.RecordObject(controller, "Speed Up Camera");
        controller.moveSpeed = cameraSpeed;
        controller.moveSmoothTime = 0.05f; // Cok responsive
        controller.heightSpeed = 150f;
        controller.minHeight = 30f;
        controller.maxHeight = 200f;
        
        // Harita sinirlari
        controller.minX = -groundSize.x / 2 - 50;
        controller.maxX = groundSize.x / 2 + 50;
        controller.minZ = -groundSize.y / 2 - 50;
        controller.maxZ = groundSize.y / 2 + 50;
        controller.useBounds = true;
        
        // Kamera pozisyonu
        Undo.RecordObject(mainCam.transform, "Position Camera");
        mainCam.transform.position = new Vector3(0, 100, -groundSize.y / 3);
        mainCam.transform.rotation = Quaternion.Euler(45, 0, 0);
        mainCam.transform.localScale = Vector3.one;
        
        Debug.Log($"[MapRestructure] Kamera hizi: {cameraSpeed}");
    }

    void UpdatePathNodes()
    {
        // Tum PathNode'larin end noktasini base'e yonlendir
        PathNode[] nodes = FindObjectsByType<PathNode>(FindObjectsSortMode.None);
        
        foreach (PathNode node in nodes)
        {
            if (node.isEnd)
            {
                // End node'u base pozisyonuna yaklaştir
                Undo.RecordObject(node.transform, "Update PathNode");
                Vector3 pos = node.transform.position;
                pos.z = Mathf.Lerp(pos.z, basePosition.z - 10, 0.5f);
                node.transform.position = pos;
            }
        }
        
        Debug.Log($"[MapRestructure] {nodes.Length} PathNode guncellendi!");
    }
}
