using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// UI temizleme ve duzeltme araci
/// Ust uste binen panelleri duzeltir
/// </summary>
public class UICleanupFixer : EditorWindow
{
    [MenuItem("Tools/UI Cleanup Fixer (Panel Duzeltici)")]
    static void Init()
    {
        UICleanupFixer window = GetWindow<UICleanupFixer>("UI Cleanup");
        window.minSize = new Vector2(400, 400);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("UI Temizleme ve Duzeltme", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Bu arac:\n" +
            "1. Tum duplicate panelleri siler\n" +
            "2. Panelleri kapatir (SetActive false)\n" +
            "3. Ground kontrolu yapar",
            MessageType.Info
        );

        GUILayout.Space(20);

        // Ana butonlar
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("TUM DUPLICATE PANELLERI SIL", GUILayout.Height(40)))
        {
            DeleteAllDuplicatePanels();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("TUM PANELLERI KAPAT (SetActive false)", GUILayout.Height(35)))
        {
            CloseAllPanels();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Ayri Islemler", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("GameOver Kapat", GUILayout.Height(30)))
        {
            ClosePanel("GameOverPanel");
        }
        if (GUILayout.Button("Victory Kapat", GUILayout.Height(30)))
        {
            ClosePanel("VictoryPanel");
        }
        if (GUILayout.Button("WaveCompleted Kapat", GUILayout.Height(30)))
        {
            ClosePanel("WaveCompletedPanel");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        EditorGUILayout.LabelField("Ground Kontrolu", EditorStyles.boldLabel);

        if (GUILayout.Button("Ground Var Mi Kontrol Et", GUILayout.Height(30)))
        {
            CheckGround();
        }

        if (GUILayout.Button("Basit Ground Olustur (1000x1000)", GUILayout.Height(30)))
        {
            CreateSimpleGround();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Time.timeScale = 1 (Oyun Devam)", GUILayout.Height(25)))
        {
            Time.timeScale = 1f;
            Debug.Log("[UIFixer] Time.timeScale = 1");
        }
    }

    void DeleteAllDuplicatePanels()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        // Panel isimleri ve sayilari
        Dictionary<string, List<GameObject>> panelCounts = new Dictionary<string, List<GameObject>>();
        string[] panelNames = { "GameOverPanel", "VictoryPanel", "WaveCompletedPanel", 
            "ScorePanel", "WavePanel", "BaseHealthPanel", "CoinPanel" };

        foreach (Canvas canvas in canvases)
        {
            Transform[] allChildren = canvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t == null || t.gameObject == null) continue;
                
                foreach (string pn in panelNames)
                {
                    if (t.name == pn || t.name.StartsWith(pn))
                    {
                        if (!panelCounts.ContainsKey(pn))
                            panelCounts[pn] = new List<GameObject>();
                        panelCounts[pn].Add(t.gameObject);
                    }
                }
            }
        }

        // Duplikat olan her seyi sil, birini birak
        int deleted = 0;
        foreach (var kvp in panelCounts)
        {
            if (kvp.Value.Count > 1)
            {
                // Ilk birini birak, gerisini sil
                for (int i = 1; i < kvp.Value.Count; i++)
                {
                    if (kvp.Value[i] != null)
                    {
                        Undo.DestroyObjectImmediate(kvp.Value[i]);
                        deleted++;
                    }
                }
                Debug.Log($"[UIFixer] {kvp.Key}: {kvp.Value.Count - 1} duplikat silindi");
            }
        }

        if (deleted > 0)
        {
            EditorUtility.DisplayDialog("Silindi", $"{deleted} duplikat panel silindi!", "Tamam");
        }
        else
        {
            EditorUtility.DisplayDialog("Temiz", "Duplikat panel bulunamadi!", "Tamam");
        }
    }

    void CloseAllPanels()
    {
        string[] panelNames = { "GameOverPanel", "VictoryPanel", "WaveCompletedPanel" };
        
        foreach (string pn in panelNames)
        {
            ClosePanel(pn);
        }
        
        Debug.Log("[UIFixer] Tum paneller kapatildi!");
    }

    void ClosePanel(string panelName)
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == panelName || obj.name.StartsWith(panelName))
            {
                Undo.RecordObject(obj, "Close Panel");
                obj.SetActive(false);
                Debug.Log($"[UIFixer] {obj.name} kapatildi");
            }
        }
    }

    void CheckGround()
    {
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        int groundCount = 0;
        
        foreach (Renderer r in allRenderers)
        {
            if (r.gameObject.name.Contains("Ground") || r.gameObject.name.Contains("Plane"))
            {
                groundCount++;
                Debug.Log($"[UIFixer] Ground bulundu: {r.gameObject.name} @ {r.transform.position}");
            }
        }

        if (groundCount == 0)
        {
            EditorUtility.DisplayDialog("Ground Yok!", 
                "Sahnede ground bulunamadi!\n\n" +
                "'Basit Ground Olustur' butonuna bas.", "Tamam");
        }
        else
        {
            EditorUtility.DisplayDialog("Ground Var", 
                $"{groundCount} ground objesi bulundu.", "Tamam");
        }
    }

    void CreateSimpleGround()
    {
        // Mevcut MainGround varsa silme
        GameObject existing = GameObject.Find("MainGround");
        if (existing != null)
        {
            Undo.DestroyObjectImmediate(existing);
        }

        // Basit plane olustur
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "MainGround";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(100, 1, 100); // 1000x1000

        // Material ata
        string[] guids = AssetDatabase.FindAssets("GrassMat t:Material");
        if (guids.Length > 0)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guids[0]));
            if (mat != null)
            {
                ground.GetComponent<Renderer>().sharedMaterial = mat;
            }
        }
        else
        {
            // Yesil material
            Material greenMat = new Material(Shader.Find("Standard"));
            greenMat.color = new Color(0.2f, 0.5f, 0.2f);
            ground.GetComponent<Renderer>().sharedMaterial = greenMat;
        }

        Undo.RegisterCreatedObjectUndo(ground, "Create Ground");
        Debug.Log("[UIFixer] 1000x1000 ground olusturuldu!");
        
        EditorUtility.DisplayDialog("Ground Olusturuldu", 
            "1000x1000 boyutunda ground olusturuldu!\n" +
            "Ctrl+S ile kaydedin.", "Tamam");
    }
}
