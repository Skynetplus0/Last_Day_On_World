using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Level 2 sahne analiz aracı
/// - Harita sınırlarını otomatik hesaplar
/// - Spawn noktalarını kontrol eder
/// - Kamera için önerilen pozisyonu hesaplar
/// </summary>
public class Level2SceneAnalyzer : EditorWindow
{
    private Vector3 mapMin;
    private Vector3 mapMax;
    private Vector3 mapCenter;
    private Vector3 suggestedCameraPos;
    private bool analyzed = false;
    
    private List<GameObject> groundObjects = new List<GameObject>();
    private List<GameObject> pathNodes = new List<GameObject>();
    private List<GameObject> spawnPoints = new List<GameObject>();

    [MenuItem("Tools/Level 2 Scene Analyzer")]
    static void Init()
    {
        Level2SceneAnalyzer window = GetWindow<Level2SceneAnalyzer>("Level 2 Analyzer");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Level 2 Sahne Analiz Aracı", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Bu araç Level 2 sahnesini analiz eder:\n" +
            "• Harita sınırlarını hesaplar\n" +
            "• Spawn noktalarını kontrol eder\n" +
            "• Kamera için önerilen pozisyonu bulur",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("SAHNEYİ ANALİZ ET", GUILayout.Height(40)))
        {
            AnalyzeScene();
        }

        GUILayout.Space(20);

        if (analyzed)
        {
            DrawAnalysisResults();
        }
    }

    void AnalyzeScene()
    {
        groundObjects.Clear();
        pathNodes.Clear();
        spawnPoints.Clear();

        mapMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        mapMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        // Tüm Ground objelerini bul
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            // Ground objelerini bul
            if (obj.name.Contains("Ground"))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    groundObjects.Add(obj);
                    
                    // Bounds hesapla
                    Bounds bounds = renderer.bounds;
                    mapMin = Vector3.Min(mapMin, bounds.min);
                    mapMax = Vector3.Max(mapMax, bounds.max);
                }
            }

            // PathNode objelerini bul
            if (obj.GetComponent<PathNode>() != null)
            {
                pathNodes.Add(obj);
            }

            // Spawner objelerini bul
            if (obj.GetComponent<Level2EnemySpawner>() != null)
            {
                spawnPoints.Add(obj);
            }
        }

        // Harita merkezi
        mapCenter = (mapMin + mapMax) / 2f;

        // Önerilen kamera pozisyonu
        float mapWidth = mapMax.x - mapMin.x;
        float mapDepth = mapMax.z - mapMin.z;
        float cameraHeight = Mathf.Max(mapWidth, mapDepth) * 0.4f; // Harita boyutuna göre yükseklik
        cameraHeight = Mathf.Clamp(cameraHeight, 20f, 80f);

        suggestedCameraPos = new Vector3(
            mapCenter.x,
            cameraHeight,
            mapCenter.z - mapDepth * 0.3f // Biraz geride
        );

        analyzed = true;

        Debug.Log($"[Analyzer] Analiz tamamlandı:\n" +
                  $"Ground sayısı: {groundObjects.Count}\n" +
                  $"PathNode sayısı: {pathNodes.Count}\n" +
                  $"Harita sınırları: {mapMin} - {mapMax}");
    }

    void DrawAnalysisResults()
    {
        EditorGUILayout.LabelField("📊 ANALİZ SONUÇLARI", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Harita sınırları
        EditorGUILayout.LabelField("Harita Sınırları", EditorStyles.boldLabel);
        EditorGUILayout.Vector3Field("Min", mapMin);
        EditorGUILayout.Vector3Field("Max", mapMax);
        EditorGUILayout.Vector3Field("Merkez", mapCenter);

        EditorGUILayout.Space();

        // Obje sayıları
        EditorGUILayout.LabelField("Bulunan Objeler", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Ground Objeleri: {groundObjects.Count}");
        EditorGUILayout.LabelField($"PathNode Objeleri: {pathNodes.Count}");
        EditorGUILayout.LabelField($"Enemy Spawner: {spawnPoints.Count}");

        EditorGUILayout.Space();

        // Önerilen kamera pozisyonu
        EditorGUILayout.LabelField("🎥 Önerilen Kamera Ayarları", EditorStyles.boldLabel);
        EditorGUILayout.Vector3Field("Position", suggestedCameraPos);
        EditorGUILayout.Vector3Field("Rotation", new Vector3(45f, 0f, 0f));
        EditorGUILayout.Vector3Field("Scale", Vector3.one);

        EditorGUILayout.Space();

        // Aksiyon butonları
        EditorGUILayout.LabelField("⚡ Hızlı Aksiyonlar", EditorStyles.boldLabel);

        if (GUILayout.Button("Kamerayı Önerilen Pozisyona Taşı", GUILayout.Height(30)))
        {
            ApplySuggestedCameraPosition();
        }

        if (GUILayout.Button("Level2CameraController Ekle/Güncelle", GUILayout.Height(30)))
        {
            SetupCameraController();
        }

        EditorGUILayout.Space();

        // Kamera sınırları için değerler
        EditorGUILayout.LabelField("📐 Level2CameraController Sınırları", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Min X: {mapMin.x - 20f:F1}");
        EditorGUILayout.LabelField($"Max X: {mapMax.x + 20f:F1}");
        EditorGUILayout.LabelField($"Min Z: {mapMin.z - 40f:F1}");
        EditorGUILayout.LabelField($"Max Z: {mapMax.z + 20f:F1}");

        if (GUILayout.Button("Sınırları Kamera Controller'a Uygula", GUILayout.Height(30)))
        {
            ApplyBoundsToController();
        }
    }

    void ApplySuggestedCameraPosition()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            EditorUtility.DisplayDialog("Hata", "Main Camera bulunamadı!", "Tamam");
            return;
        }

        Undo.RecordObject(mainCam.transform, "Move Camera");
        
        mainCam.transform.position = suggestedCameraPos;
        mainCam.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
        mainCam.transform.localScale = Vector3.one;

        EditorUtility.DisplayDialog("Başarılı", 
            $"Kamera pozisyonu güncellendi:\n" +
            $"Position: {suggestedCameraPos}\n" +
            $"Rotation: (45, 0, 0)\n" +
            $"Scale: (1, 1, 1)", "Tamam");
    }

    void SetupCameraController()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            EditorUtility.DisplayDialog("Hata", "Main Camera bulunamadı!", "Tamam");
            return;
        }

        // Eski CameraController varsa kaldır
        CameraController oldController = mainCam.GetComponent<CameraController>();
        if (oldController != null)
        {
            Undo.DestroyObjectImmediate(oldController);
        }

        // Level2CameraController ekle veya al
        Level2CameraController newController = mainCam.GetComponent<Level2CameraController>();
        if (newController == null)
        {
            newController = Undo.AddComponent<Level2CameraController>(mainCam.gameObject);
        }

        // Sınırları ayarla
        Undo.RecordObject(newController, "Setup Camera Controller");
        newController.minX = mapMin.x - 20f;
        newController.maxX = mapMax.x + 20f;
        newController.minZ = mapMin.z - 40f;
        newController.maxZ = mapMax.z + 20f;
        newController.minHeight = 15f;
        newController.maxHeight = 80f;
        newController.useBounds = true;

        EditorUtility.DisplayDialog("Başarılı",
            "Level2CameraController eklendi/güncellendi!\n" +
            "Eski CameraController kaldırıldı (varsa).", "Tamam");
    }

    void ApplyBoundsToController()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            EditorUtility.DisplayDialog("Hata", "Main Camera bulunamadı!", "Tamam");
            return;
        }

        Level2CameraController controller = mainCam.GetComponent<Level2CameraController>();
        if (controller == null)
        {
            EditorUtility.DisplayDialog("Hata", 
                "Level2CameraController bulunamadı!\n" +
                "Önce 'Level2CameraController Ekle' butonunu kullanın.", "Tamam");
            return;
        }

        Undo.RecordObject(controller, "Apply Bounds");
        controller.minX = mapMin.x - 20f;
        controller.maxX = mapMax.x + 20f;
        controller.minZ = mapMin.z - 40f;
        controller.maxZ = mapMax.z + 20f;
        controller.useBounds = true;

        EditorUtility.DisplayDialog("Başarılı", "Sınırlar uygulandı!", "Tamam");
    }
}
