using UnityEngine;
using UnityEditor;

/// <summary>
/// Ground ve Road objelerine material atama aracı
/// Tek tıkla tüm Ground ve Road objelerine material atar
/// </summary>
public class GroundMaterialAssigner : EditorWindow
{
    public Material groundMaterial;
    public Material roadMaterial;
    
    private int groundCount = 0;
    private int roadCount = 0;

    [MenuItem("Tools/Ground & Road Material Assigner")]
    static void Init()
    {
        GroundMaterialAssigner window = GetWindow<GroundMaterialAssigner>("Material Assigner");
        window.minSize = new Vector2(350, 300);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Ground & Road Material Atama Aracı", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Bu araç sahnedeki tüm 'Ground' ve 'Road' objelerine\n" +
            "otomatik olarak material atar.",
            MessageType.Info
        );

        GUILayout.Space(15);

        // Ground Material
        EditorGUILayout.LabelField("🌿 Ground Material", EditorStyles.boldLabel);
        groundMaterial = (Material)EditorGUILayout.ObjectField("Material:", groundMaterial, typeof(Material), false);
        
        if (GUILayout.Button("TÜM GROUND OBJELERİNE UYGULA", GUILayout.Height(35)))
        {
            ApplyMaterialToObjects("Ground", groundMaterial, ref groundCount);
        }
        EditorGUILayout.LabelField($"Son işlem: {groundCount} Ground objesi güncellendi");

        GUILayout.Space(20);

        // Road Material
        EditorGUILayout.LabelField("🛣️ Road Material", EditorStyles.boldLabel);
        roadMaterial = (Material)EditorGUILayout.ObjectField("Material:", roadMaterial, typeof(Material), false);
        
        if (GUILayout.Button("TÜM ROAD OBJELERİNE UYGULA", GUILayout.Height(35)))
        {
            ApplyMaterialToObjects("Road", roadMaterial, ref roadCount);
        }
        EditorGUILayout.LabelField($"Son işlem: {roadCount} Road objesi güncellendi");

        GUILayout.Space(20);

        // Her ikisini birden uygula
        EditorGUILayout.LabelField("⚡ Hızlı Uygulama", EditorStyles.boldLabel);
        if (GUILayout.Button("HER İKİSİNİ BİRDEN UYGULA", GUILayout.Height(40)))
        {
            if (groundMaterial != null)
                ApplyMaterialToObjects("Ground", groundMaterial, ref groundCount);
            if (roadMaterial != null)
                ApplyMaterialToObjects("Road", roadMaterial, ref roadCount);
            
            EditorUtility.DisplayDialog("Tamamlandı", 
                $"Ground: {groundCount} obje\nRoad: {roadCount} obje\n\nMaterial uygulandı!", 
                "Tamam");
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("NOT: İşlemden sonra Ctrl+S ile sahneyi kaydedin.", EditorStyles.miniLabel);
    }

    void ApplyMaterialToObjects(string nameContains, Material material, ref int count)
    {
        if (material == null)
        {
            EditorUtility.DisplayDialog("Hata", $"Lütfen bir {nameContains} material seçin!", "Tamam");
            return;
        }

        count = 0;
        
        // Sahnedeki tüm GameObjectleri bul
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        
        foreach (GameObject obj in allObjects)
        {
            // İsmi kontrol et
            if (obj.name.Contains(nameContains))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Undo.RecordObject(renderer, $"Change {nameContains} Material");
                    renderer.sharedMaterial = material;
                    count++;
                }
            }
        }
        
        Debug.Log($"[MaterialAssigner] {count} adet {nameContains} objesine material atandı.");
    }
}
