using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Level 2 icin random prop yerlestime araci
/// Agaclar, kayalar, evler, lambalar, vb.
/// </summary>
public class RandomPropPlacer : EditorWindow
{
    // Ayarlar
    private int treeCount = 15;
    private int rockCount = 8;
    private int houseCount = 3;
    private int lightCount = 6;
    private int grassCount = 20;
    
    private float minX = -200f;
    private float maxX = 50f;
    private float minZ = -150f;
    private float maxZ = 50f;
    private float groundY = 0f;
    
    private bool avoidRoads = true;
    private float roadAvoidDistance = 5f;

    [MenuItem("Tools/Random Prop Placer (Level 2)")]
    static void Init()
    {
        RandomPropPlacer window = GetWindow<RandomPropPlacer>("Prop Placer");
        window.minSize = new Vector2(400, 550);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Random Prop Yerlestime Araci", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Level 2'ye random objeler yerlestirir:\n" +
            "- Agaclar (polygonTrees)\n" +
            "- Kayalar (Mountain Terrain)\n" +
            "- Evler (ALP_Assets)\n" +
            "- Isiklar\n" +
            "- Cimenler",
            MessageType.Info
        );

        GUILayout.Space(15);

        // Prop sayilari
        EditorGUILayout.LabelField("Prop Sayilari", EditorStyles.boldLabel);
        treeCount = EditorGUILayout.IntSlider("Agac:", treeCount, 0, 50);
        rockCount = EditorGUILayout.IntSlider("Kaya:", rockCount, 0, 30);
        houseCount = EditorGUILayout.IntSlider("Ev:", houseCount, 0, 10);
        lightCount = EditorGUILayout.IntSlider("Isik:", lightCount, 0, 15);
        grassCount = EditorGUILayout.IntSlider("Cimen:", grassCount, 0, 50);

        GUILayout.Space(15);

        // Alan sinirlari
        EditorGUILayout.LabelField("Yerlesim Alani", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("X:", GUILayout.Width(20));
        minX = EditorGUILayout.FloatField(minX, GUILayout.Width(60));
        EditorGUILayout.LabelField("-", GUILayout.Width(15));
        maxX = EditorGUILayout.FloatField(maxX, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Z:", GUILayout.Width(20));
        minZ = EditorGUILayout.FloatField(minZ, GUILayout.Width(60));
        EditorGUILayout.LabelField("-", GUILayout.Width(15));
        maxZ = EditorGUILayout.FloatField(maxZ, GUILayout.Width(60));
        EditorGUILayout.EndHorizontal();
        
        groundY = EditorGUILayout.FloatField("Zemin Y:", groundY);

        GUILayout.Space(10);
        avoidRoads = EditorGUILayout.Toggle("Yollardan kacin:", avoidRoads);
        if (avoidRoads)
            roadAvoidDistance = EditorGUILayout.FloatField("Yol mesafesi:", roadAvoidDistance);

        GUILayout.Space(15);

        // Harita sinirlarini otomatik al
        if (GUILayout.Button("Harita Sinirlarini Otomatik Al", GUILayout.Height(25)))
        {
            AutoDetectBounds();
        }

        GUILayout.Space(15);

        // Ana butonlar
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("TUM PROPLARI YERLESTIR", GUILayout.Height(45)))
        {
            PlaceAllProps();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        // Tek tek butonlar
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Agaclar", GUILayout.Height(30)))
        {
            PlaceTrees();
        }
        if (GUILayout.Button("Kayalar", GUILayout.Height(30)))
        {
            PlaceRocks();
        }
        if (GUILayout.Button("Evler", GUILayout.Height(30)))
        {
            PlaceHouses();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Isiklar", GUILayout.Height(30)))
        {
            PlaceLights();
        }
        if (GUILayout.Button("Cimenler", GUILayout.Height(30)))
        {
            PlaceGrass();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("TUM PROPLARI TEMIZLE", GUILayout.Height(30)))
        {
            CleanupProps();
        }
        GUI.backgroundColor = Color.white;
    }

    void AutoDetectBounds()
    {
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        Vector3 min = new Vector3(float.MaxValue, 0, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, 0, float.MinValue);
        
        foreach (Renderer r in renderers)
        {
            if (r.gameObject.name.Contains("Ground"))
            {
                min = Vector3.Min(min, r.bounds.min);
                max = Vector3.Max(max, r.bounds.max);
            }
        }

        if (min.x < float.MaxValue)
        {
            minX = min.x + 10f;
            maxX = max.x - 10f;
            minZ = min.z + 10f;
            maxZ = max.z - 10f;
            groundY = min.y;
            Debug.Log($"[PropPlacer] Sinirlar: X({minX}, {maxX}) Z({minZ}, {maxZ}) Y:{groundY}");
        }
    }

    void PlaceAllProps()
    {
        CleanupProps();
        PlaceTrees();
        PlaceRocks();
        PlaceHouses();
        PlaceLights();
        PlaceGrass();
        
        EditorUtility.DisplayDialog("Tamamlandi", 
            $"Proplar yerlestirildi:\n" +
            $"- Agac: {treeCount}\n" +
            $"- Kaya: {rockCount}\n" +
            $"- Ev: {houseCount}\n" +
            $"- Isik: {lightCount}\n" +
            $"- Cimen: {grassCount}", "Tamam");
    }

    void CleanupProps()
    {
        string[] containers = { "Props_Trees", "Props_Rocks", "Props_Houses", "Props_Lights", "Props_Grass" };
        foreach (string name in containers)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Undo.DestroyObjectImmediate(obj);
        }
        Debug.Log("[PropPlacer] Proplar temizlendi!");
    }

    void PlaceTrees()
    {
        if (treeCount == 0) return;

        // Tree prefab'larini bul
        string[] guids = AssetDatabase.FindAssets("tree t:Prefab", new[] { "Assets/polygonTrees" });
        if (guids.Length == 0)
        {
            Debug.LogWarning("[PropPlacer] Agac prefab'i bulunamadi!");
            return;
        }

        List<GameObject> prefabs = new List<GameObject>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("Variant") && !path.Contains("meshCollider"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null) prefabs.Add(prefab);
            }
        }

        if (prefabs.Count == 0)
        {
            Debug.LogWarning("[PropPlacer] Uygun agac prefab'i bulunamadi!");
            return;
        }

        GameObject container = GetOrCreateContainer("Props_Trees");
        
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            if (!IsPositionValid(pos)) { i--; continue; }
            
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject tree = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
            tree.transform.position = pos;
            tree.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            tree.transform.localScale = Vector3.one * Random.Range(0.8f, 1.3f);
            Undo.RegisterCreatedObjectUndo(tree, "Place Tree");
        }

        Debug.Log($"[PropPlacer] {treeCount} agac yerlestirildi!");
    }

    void PlaceRocks()
    {
        if (rockCount == 0) return;

        string[] guids = AssetDatabase.FindAssets("rock_set t:Prefab", new[] { "Assets/Mountain Terrain rocks and tree" });
        if (guids.Length == 0)
        {
            Debug.LogWarning("[PropPlacer] Kaya prefab'i bulunamadi!");
            return;
        }

        List<GameObject> prefabs = new List<GameObject>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) prefabs.Add(prefab);
        }

        GameObject container = GetOrCreateContainer("Props_Rocks");
        
        for (int i = 0; i < rockCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            if (!IsPositionValid(pos)) { i--; continue; }
            
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject rock = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
            rock.transform.position = pos;
            rock.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            rock.transform.localScale = Vector3.one * Random.Range(0.5f, 1.5f);
            Undo.RegisterCreatedObjectUndo(rock, "Place Rock");
        }

        Debug.Log($"[PropPlacer] {rockCount} kaya yerlestirildi!");
    }

    void PlaceHouses()
    {
        if (houseCount == 0) return;

        string[] guids = AssetDatabase.FindAssets("House t:Prefab", new[] { "Assets/ALP_Assets" });
        if (guids.Length == 0)
        {
            Debug.LogWarning("[PropPlacer] Ev prefab'i bulunamadi!");
            return;
        }

        List<GameObject> prefabs = new List<GameObject>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null) prefabs.Add(prefab);
        }

        GameObject container = GetOrCreateContainer("Props_Houses");
        
        // Evleri harita genelinde dagit
        for (int i = 0; i < houseCount; i++)
        {
            // Evleri haritanin farkli bolumlerine dagit
            float x = Random.Range(minX + 20f, maxX - 20f);
            float z = Random.Range(minZ + 20f, maxZ - 20f);
            Vector3 pos = new Vector3(x, groundY, z);
            
            // Yollardan uzak olsun
            int attempts = 0;
            while (!IsPositionValid(pos) && attempts < 10)
            {
                x = Random.Range(minX + 20f, maxX - 20f);
                z = Random.Range(minZ + 20f, maxZ - 20f);
                pos = new Vector3(x, groundY, z);
                attempts++;
            }
            
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject house = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
            house.transform.position = pos;
            house.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            house.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
            
            // HouseHealth component ekle
            if (house.GetComponent<HouseHealth>() == null)
            {
                house.AddComponent<HouseHealth>();
            }
            
            Undo.RegisterCreatedObjectUndo(house, "Place House");
        }

        Debug.Log($"[PropPlacer] {houseCount} ev yerlestirildi!");
    }

    void PlaceLights()
    {
        if (lightCount == 0) return;

        GameObject container = GetOrCreateContainer("Props_Lights");
        
        for (int i = 0; i < lightCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            pos.y = groundY + 6f; // Havada
            
            GameObject lightObj = new GameObject($"StreetLight_{i}");
            lightObj.transform.parent = container.transform;
            lightObj.transform.position = pos;
            
            Light pointLight = lightObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.color = new Color(1f, 0.9f, 0.7f);
            pointLight.intensity = 1.5f;
            pointLight.range = 18f;
            pointLight.shadows = LightShadows.None;
            
            Undo.RegisterCreatedObjectUndo(lightObj, "Place Light");
        }

        Debug.Log($"[PropPlacer] {lightCount} isik yerlestirildi!");
    }

    void PlaceGrass()
    {
        if (grassCount == 0) return;

        string[] guids = AssetDatabase.FindAssets("littleGrass t:Prefab", new[] { "Assets/polygonTrees" });
        if (guids.Length == 0)
        {
            Debug.LogWarning("[PropPlacer] Cimen prefab'i bulunamadi!");
            return;
        }

        List<GameObject> prefabs = new List<GameObject>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.Contains("meshCollider"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null) prefabs.Add(prefab);
            }
        }

        if (prefabs.Count == 0) return;

        GameObject container = GetOrCreateContainer("Props_Grass");
        
        for (int i = 0; i < grassCount; i++)
        {
            Vector3 pos = GetRandomPosition();
            if (!IsPositionValid(pos)) { i--; continue; }
            
            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];
            GameObject grass = (GameObject)PrefabUtility.InstantiatePrefab(prefab, container.transform);
            grass.transform.position = pos;
            grass.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            grass.transform.localScale = Vector3.one * Random.Range(0.7f, 1.5f);
            Undo.RegisterCreatedObjectUndo(grass, "Place Grass");
        }

        Debug.Log($"[PropPlacer] {grassCount} cimen yerlestirildi!");
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(minX, maxX);
        float z = Random.Range(minZ, maxZ);
        return new Vector3(x, groundY, z);
    }

    bool IsPositionValid(Vector3 pos)
    {
        if (!avoidRoads) return true;

        // Yollardan uzak mi kontrol et
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Road"))
            {
                float dist = Vector3.Distance(pos, obj.transform.position);
                if (dist < roadAvoidDistance) return false;
            }
        }
        return true;
    }

    GameObject GetOrCreateContainer(string name)
    {
        GameObject container = GameObject.Find(name);
        if (container == null)
        {
            container = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(container, "Create Container");
        }
        return container;
    }
}
