using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Tum objelere material atama araci
/// Ground, Road, ve diger objelere toplu material atama
/// </summary>
public class UniversalMaterialAssigner : EditorWindow
{
    private Material selectedMaterial;
    private string searchPattern = "Ground";
    private List<GameObject> foundObjects = new List<GameObject>();
    private Vector2 scrollPosition;

    [MenuItem("Tools/Universal Material Assigner")]
    static void Init()
    {
        UniversalMaterialAssigner window = GetWindow<UniversalMaterialAssigner>("Material Assigner");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Universal Material Atama Araci", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Isim iceren tum objelere material atar.\n" +
            "Ornek: 'Ground' yazin, tum Ground objelerine atar.",
            MessageType.Info
        );

        GUILayout.Space(15);

        // Arama pattern'i
        EditorGUILayout.LabelField("Arama Ayarlari", EditorStyles.boldLabel);
        searchPattern = EditorGUILayout.TextField("Isim iceren:", searchPattern);
        
        GUILayout.Space(10);

        if (GUILayout.Button("OBJELERI BUL", GUILayout.Height(30)))
        {
            FindObjects();
        }

        GUILayout.Space(10);

        // Bulunan objeler
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.LabelField($"Bulunan: {foundObjects.Count} obje", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
            foreach (var obj in foundObjects)
            {
                if (obj != null)
                {
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(15);

        // Material secimi
        EditorGUILayout.LabelField("Material Sec", EditorStyles.boldLabel);
        selectedMaterial = (Material)EditorGUILayout.ObjectField("Material:", selectedMaterial, typeof(Material), false);

        GUILayout.Space(10);

        // Hizli material butonlari
        EditorGUILayout.LabelField("Hizli Material Secimi", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GrassMat"))
        {
            LoadMaterial("GrassMat");
        }
        if (GUILayout.Button("GrassMatImage"))
        {
            LoadMaterial("GrassMatImage");
        }
        if (GUILayout.Button("BlackWayMat"))
        {
            LoadMaterial("BlackWayMat");
        }
        if (GUILayout.Button("SoilMat"))
        {
            LoadMaterial("SoilMat");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Uygula butonu
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("MATERIAL UYGULA", GUILayout.Height(45)))
        {
            ApplyMaterial();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(15);

        // Hazir presetler
        EditorGUILayout.LabelField("Hazir Presetler", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ground -> Grass", GUILayout.Height(30)))
        {
            searchPattern = "Ground";
            LoadMaterial("GrassMat");
            FindObjects();
            ApplyMaterial();
        }
        if (GUILayout.Button("Road -> Black", GUILayout.Height(30)))
        {
            searchPattern = "Road";
            LoadMaterial("BlackWayMat");
            FindObjects();
            ApplyMaterial();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Ctrl+S ile kaydedin!", EditorStyles.miniLabel);
    }

    void FindObjects()
    {
        foundObjects.Clear();
        
        if (string.IsNullOrEmpty(searchPattern))
        {
            EditorUtility.DisplayDialog("Hata", "Lutfen bir arama pattern'i girin!", "Tamam");
            return;
        }

        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(searchPattern))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    foundObjects.Add(obj);
                }
            }
        }

        Debug.Log($"[MaterialAssigner] {foundObjects.Count} obje bulundu: '{searchPattern}'");
    }

    void LoadMaterial(string materialName)
    {
        string[] guids = AssetDatabase.FindAssets($"{materialName} t:Material");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            selectedMaterial = AssetDatabase.LoadAssetAtPath<Material>(path);
            Debug.Log($"[MaterialAssigner] Material yuklendi: {materialName}");
        }
        else
        {
            Debug.LogWarning($"[MaterialAssigner] Material bulunamadi: {materialName}");
        }
    }

    void ApplyMaterial()
    {
        if (selectedMaterial == null)
        {
            EditorUtility.DisplayDialog("Hata", "Lutfen bir material secin!", "Tamam");
            return;
        }

        if (foundObjects.Count == 0)
        {
            FindObjects();
        }

        if (foundObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Hata", "Hic obje bulunamadi!", "Tamam");
            return;
        }

        int count = 0;
        foreach (GameObject obj in foundObjects)
        {
            if (obj == null) continue;
            
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Undo.RecordObject(renderer, "Apply Material");
                renderer.sharedMaterial = selectedMaterial;
                count++;
            }
        }

        EditorUtility.DisplayDialog("Tamamlandi", 
            $"{count} objeye '{selectedMaterial.name}' uyguland!", "Tamam");
        
        Debug.Log($"[MaterialAssigner] {count} objeye material uyguland!");
    }
}
