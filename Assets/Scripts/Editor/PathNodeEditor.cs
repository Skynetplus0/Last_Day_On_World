using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Default inspector'ı çiz (mevcut alanlar görünsün)
        base.OnInspectorGUI();

        PathNode node = (PathNode)target;

        GUILayout.Space(20);

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 14;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.fixedHeight = 40;

        if (GUILayout.Button("Create Next Node (+)", buttonStyle))
        {
            CreateNextNode(node);
        }
    }

    void CreateNextNode(PathNode currentNode)
    {
        // Yeni GameObject oluştur
        GameObject g = new GameObject("PathNode_" + (currentNode.nextNodes.Count + 1));
        
        // Konumunu ayarla (biraz ileriye koy)
        // Eğer currentNode'un zaten bir sonraki node'u varsa, onun doğrultusunda değilse forward yönünde koy
        Vector3 spawnPos = currentNode.transform.position + Vector3.forward * 3f;
        // Sahne kamerasının baktığı yere göre falan değil, standart forward. 
        // Kullanıcı zaten move tool ile taşıyacak.

        g.transform.position = spawnPos;
        g.transform.rotation = currentNode.transform.rotation;

        // PathNode bileşeni ekle
        PathNode newNode = g.AddComponent<PathNode>();

        // Undo desteği (CTRL+Z ile geri alınabilsin diye)
        Undo.RegisterCreatedObjectUndo(g, "Create Path Node");
        Undo.RecordObject(currentNode, "Link Path Node");

        // Bağlantıyı kur
        currentNode.nextNodes.Add(newNode);
        
        // Validate'in çalışmasını tetikle veya manuel ekle
        // PathNode.cs OnValidate'de weightleri eşitliyor ama biz de ekleyelim
        if (currentNode.childWeights != null)
        {
             currentNode.childWeights.Add(1f);
        }

        // Değişikliği Unity'ye bildir
        EditorUtility.SetDirty(currentNode);

        // Yeni oluşturulan node'u seç ki hemen taşımaya başlayabilsin
        Selection.activeGameObject = g;
    }
}
