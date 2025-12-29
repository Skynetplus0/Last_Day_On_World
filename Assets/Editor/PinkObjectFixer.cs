using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Pembe (material eksik) objeleri ve performans sorunlarini duzeltir
/// </summary>
public class PinkObjectFixer : EditorWindow
{
    private List<GameObject> pinkObjects = new List<GameObject>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Pink Object Fixer (Pembe Obje Silici)")]
    static void Init()
    {
        PinkObjectFixer window = GetWindow<PinkObjectFixer>("Pink Fixer");
        window.minSize = new Vector2(400, 400);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Pembe Obje Duzeltici", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Material eksik (pembe/mor) objeleri bulur ve siler.\n" +
            "Bu objeler genellikle prefab'larin URP uyumsuzlugundan kaynaklanir.\n" +
            "En iyi cozum: silip yerine Level 1'deki objeleri koymak.",
            MessageType.Warning
        );

        GUILayout.Space(15);

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("PEMBE OBJELERI BUL", GUILayout.Height(35)))
        {
            FindPinkObjects();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        // Sonuclar
        if (pinkObjects.Count > 0)
        {
            EditorGUILayout.LabelField($"Bulunan: {pinkObjects.Count} pembe obje", EditorStyles.boldLabel);
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(120));
            foreach (var obj in pinkObjects)
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

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("TUM PEMBE OBJELERI SIL", GUILayout.Height(45)))
        {
            DeletePinkObjects();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Hizli Silme", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Props_Trees Sil", GUILayout.Height(30)))
        {
            DeleteContainer("Props_Trees");
        }
        if (GUILayout.Button("Props_Rocks Sil", GUILayout.Height(30)))
        {
            DeleteContainer("Props_Rocks");
        }
        if (GUILayout.Button("Props_Grass Sil", GUILayout.Height(30)))
        {
            DeleteContainer("Props_Grass");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Props_Houses Sil", GUILayout.Height(30)))
        {
            DeleteContainer("Props_Houses");
        }
        if (GUILayout.Button("Props_Lights Sil", GUILayout.Height(30)))
        {
            DeleteContainer("Props_Lights");
        }
        if (GUILayout.Button("TUM PROPS SIL", GUILayout.Height(30)))
        {
            DeleteAllProps();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);

        EditorGUILayout.LabelField("Performans Kontrolu", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Obje Sayisini Goster", GUILayout.Height(25)))
        {
            ShowObjectCount();
        }
    }

    void FindPinkObjects()
    {
        pinkObjects.Clear();
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        
        foreach (Renderer r in allRenderers)
        {
            if (r == null) continue;
            
            Material[] mats = r.sharedMaterials;
            foreach (Material m in mats)
            {
                if (IsPinkMaterial(m))
                {
                    if (!pinkObjects.Contains(r.gameObject))
                        pinkObjects.Add(r.gameObject);
                    break;
                }
            }
        }

        Debug.Log($"[PinkFixer] {pinkObjects.Count} pembe obje bulundu!");
        
        if (pinkObjects.Count == 0)
        {
            EditorUtility.DisplayDialog("Temiz", "Pembe obje bulunamadi!", "Tamam");
        }
    }

    bool IsPinkMaterial(Material m)
    {
        if (m == null) return true;
        if (m.shader == null) return true;
        if (m.shader.name == "Hidden/InternalErrorShader") return true;
        if (m.shader.name.Contains("Error")) return true;
        return false;
    }

    void DeletePinkObjects()
    {
        if (pinkObjects.Count == 0)
        {
            FindPinkObjects();
        }

        int count = 0;
        List<GameObject> toDelete = new List<GameObject>(pinkObjects);
        
        foreach (GameObject obj in toDelete)
        {
            if (obj != null)
            {
                Undo.DestroyObjectImmediate(obj);
                count++;
            }
        }

        pinkObjects.Clear();
        EditorUtility.DisplayDialog("Silindi", $"{count} pembe obje silindi!", "Tamam");
        Debug.Log($"[PinkFixer] {count} pembe obje silindi!");
    }

    void DeleteContainer(string name)
    {
        GameObject container = GameObject.Find(name);
        if (container != null)
        {
            int childCount = container.transform.childCount;
            Undo.DestroyObjectImmediate(container);
            Debug.Log($"[PinkFixer] {name} silindi ({childCount} obje)!");
            EditorUtility.DisplayDialog("Silindi", $"{name} silindi ({childCount} obje)!", "Tamam");
        }
        else
        {
            EditorUtility.DisplayDialog("Bulunamadi", $"{name} bulunamadi!", "Tamam");
        }
    }

    void DeleteAllProps()
    {
        string[] containers = { "Props_Trees", "Props_Rocks", "Props_Houses", "Props_Lights", "Props_Grass" };
        int totalDeleted = 0;
        
        foreach (string name in containers)
        {
            GameObject container = GameObject.Find(name);
            if (container != null)
            {
                totalDeleted += container.transform.childCount;
                Undo.DestroyObjectImmediate(container);
            }
        }

        EditorUtility.DisplayDialog("Tamamlandi", $"Tum props silindi ({totalDeleted} obje)!", "Tamam");
        Debug.Log($"[PinkFixer] Tum props silindi ({totalDeleted} obje)!");
    }

    void ShowObjectCount()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        
        string msg = $"Sahnedeki Objeler:\n\n" +
            $"Toplam GameObject: {allObjects.Length}\n" +
            $"Renderer (gorunen): {allRenderers.Length}\n" +
            $"Light: {allLights.Length}\n\n" +
            $"Performans Notu:\n" +
            $"- 1000+ obje = yavas\n" +
            $"- 100+ light = cok yavas\n" +
            $"- Onerilen: max 500 obje, max 10 light";

        EditorUtility.DisplayDialog("Obje Sayisi", msg, "Tamam");
        Debug.Log($"[PinkFixer] Objeler: {allObjects.Length}, Renderers: {allRenderers.Length}, Lights: {allLights.Length}");
    }
}
