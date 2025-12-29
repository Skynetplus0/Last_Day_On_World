using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Eksik material bulunan (mor gorunen) objeleri duzeltir
/// </summary>
public class MissingMaterialFixer : EditorWindow
{
    private Material fallbackMaterial;
    private bool autoFix = true;
    private Vector2 scrollPos;
    private List<GameObject> brokenObjects = new List<GameObject>();

    [MenuItem("Tools/Missing Material Fixer (Mor Obje Duzeltici)")]
    static void Init()
    {
        MissingMaterialFixer window = GetWindow<MissingMaterialFixer>("Material Fixer");
        window.minSize = new Vector2(400, 450);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Eksik Material Duzeltici", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Mor gorunen (material eksik) objeleri bulur ve duzeltir.\n" +
            "Ya uygun material atar ya da fallback material kullanir.",
            MessageType.Info
        );

        GUILayout.Space(15);

        // Fallback material
        EditorGUILayout.LabelField("Fallback Material (Bulamazsa)", EditorStyles.boldLabel);
        fallbackMaterial = (Material)EditorGUILayout.ObjectField("Material:", fallbackMaterial, typeof(Material), false);
        autoFix = EditorGUILayout.Toggle("Otomatik material bul:", autoFix);

        GUILayout.Space(15);

        // Hizli material yukle
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Grass Mat"))
        {
            LoadMaterial("GrassMat");
        }
        if (GUILayout.Button("Tree Mat"))
        {
            LoadMaterial("TreeMaterial");
        }
        if (GUILayout.Button("Rock Mat"))
        {
            LoadMaterial("rock_set_mat");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        // Ana butonlar
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("EKSIK MATERIAL OBJELERINI BUL", GUILayout.Height(35)))
        {
            FindBrokenObjects();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        // Bulunanlar listesi
        if (brokenObjects.Count > 0)
        {
            EditorGUILayout.LabelField($"Bulunan: {brokenObjects.Count} obje", EditorStyles.boldLabel);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(120));
            foreach (var obj in brokenObjects)
            {
                if (obj != null)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
                    if (GUILayout.Button("Sec", GUILayout.Width(40)))
                    {
                        Selection.activeGameObject = obj;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(15);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("TUM EKSIK MATERIALLERI DUZELT", GUILayout.Height(45)))
        {
            FixAllMaterials();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        // Props container'lari icin ozel butonlar
        EditorGUILayout.LabelField("Prop Container'lar Icin", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Props_Trees Duzelt", GUILayout.Height(30)))
        {
            FixContainer("Props_Trees", "TreeMaterial");
        }
        if (GUILayout.Button("Props_Grass Duzelt", GUILayout.Height(30)))
        {
            FixContainer("Props_Grass", "grassMaterial");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Props_Rocks Duzelt", GUILayout.Height(30)))
        {
            FixContainer("Props_Rocks", "rock_set_mat");
        }
        if (GUILayout.Button("Props_Houses Duzelt", GUILayout.Height(30)))
        {
            FixContainer("Props_Houses", null);
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("TUM PROPS OBJELERINI SIL", GUILayout.Height(30)))
        {
            DeleteAllProps();
        }
        GUI.backgroundColor = Color.white;
    }

    void LoadMaterial(string name)
    {
        string[] guids = AssetDatabase.FindAssets($"{name} t:Material");
        if (guids.Length > 0)
        {
            fallbackMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[0]));
            Debug.Log($"[MaterialFixer] Material yuklendi: {name}");
        }
    }

    void FindBrokenObjects()
    {
        brokenObjects.Clear();
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        
        foreach (Renderer r in allRenderers)
        {
            if (r == null) continue;
            
            Material[] mats = r.sharedMaterials;
            foreach (Material m in mats)
            {
                if (m == null)
                {
                    brokenObjects.Add(r.gameObject);
                    break;
                }
                // Shader eksik mi kontrol
                if (m.shader == null || m.shader.name == "Hidden/InternalErrorShader")
                {
                    brokenObjects.Add(r.gameObject);
                    break;
                }
            }
        }

        Debug.Log($"[MaterialFixer] {brokenObjects.Count} eksik material objesi bulundu!");
        
        if (brokenObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Sonuc", "Eksik material bulunan obje yok!", "Tamam");
        }
    }

    void FixAllMaterials()
    {
        if (brokenObjects.Count == 0)
        {
            FindBrokenObjects();
        }

        int fixedCount = 0;
        foreach (GameObject obj in brokenObjects)
        {
            if (obj == null) continue;
            if (FixObjectMaterial(obj))
            {
                fixedCount++;
            }
        }

        brokenObjects.Clear();
        EditorUtility.DisplayDialog("Tamamlandi", $"{fixedCount} obje duzeltildi!", "Tamam");
        Debug.Log($"[MaterialFixer] {fixedCount} obje duzeltildi!");
    }

    bool FixObjectMaterial(GameObject obj)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r == null) return false;

        Material newMat = null;

        if (autoFix)
        {
            // Obje ismine gore material bul
            string objName = obj.name.ToLower();
            
            if (objName.Contains("tree") || objName.Contains("branch"))
            {
                newMat = FindMaterialByName("TreeMaterial");
            }
            else if (objName.Contains("grass"))
            {
                newMat = FindMaterialByName("grassMaterial");
            }
            else if (objName.Contains("rock"))
            {
                newMat = FindMaterialByName("rock_set_mat");
            }
            else if (objName.Contains("shrub") || objName.Contains("leaves"))
            {
                newMat = FindMaterialByName("leavesMaterial");
            }
        }

        // Fallback material kullan
        if (newMat == null && fallbackMaterial != null)
        {
            newMat = fallbackMaterial;
        }

        if (newMat != null)
        {
            Undo.RecordObject(r, "Fix Material");
            
            Material[] mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null || mats[i].shader == null || mats[i].shader.name == "Hidden/InternalErrorShader")
                {
                    mats[i] = newMat;
                }
            }
            r.sharedMaterials = mats;
            return true;
        }

        return false;
    }

    Material FindMaterialByName(string name)
    {
        string[] guids = AssetDatabase.FindAssets($"{name} t:Material");
        if (guids.Length > 0)
        {
            return AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }
        return null;
    }

    void FixContainer(string containerName, string materialName)
    {
        GameObject container = GameObject.Find(containerName);
        if (container == null)
        {
            EditorUtility.DisplayDialog("Hata", $"{containerName} bulunamadi!", "Tamam");
            return;
        }

        Material mat = null;
        if (!string.IsNullOrEmpty(materialName))
        {
            mat = FindMaterialByName(materialName);
        }

        if (mat == null && fallbackMaterial != null)
        {
            mat = fallbackMaterial;
        }

        int fixedCount = 0;
        Renderer[] renderers = container.GetComponentsInChildren<Renderer>(true);
        
        foreach (Renderer r in renderers)
        {
            Material[] mats = r.sharedMaterials;
            bool needsFix = false;
            
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] == null || mats[i].shader == null || mats[i].shader.name == "Hidden/InternalErrorShader")
                {
                    if (mat != null)
                    {
                        mats[i] = mat;
                        needsFix = true;
                    }
                }
            }
            
            if (needsFix)
            {
                Undo.RecordObject(r, "Fix Container Material");
                r.sharedMaterials = mats;
                fixedCount++;
            }
        }

        EditorUtility.DisplayDialog("Tamamlandi", $"{containerName}: {fixedCount} obje duzeltildi!", "Tamam");
        Debug.Log($"[MaterialFixer] {containerName}: {fixedCount} obje duzeltildi!");
    }

    void DeleteAllProps()
    {
        if (!EditorUtility.DisplayDialog("Emin misiniz?", "Tum Props objelerini silmek istiyor musunuz?", "Evet", "Hayir"))
        {
            return;
        }

        string[] containers = { "Props_Trees", "Props_Rocks", "Props_Houses", "Props_Lights", "Props_Grass" };
        int deletedCount = 0;
        
        foreach (string name in containers)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                Undo.DestroyObjectImmediate(obj);
                deletedCount++;
            }
        }

        EditorUtility.DisplayDialog("Silindi", $"{deletedCount} container silindi!", "Tamam");
    }
}
