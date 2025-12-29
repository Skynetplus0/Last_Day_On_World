using UnityEngine;
using UnityEditor;

public class AssignMaterial : EditorWindow
{
    public Material newMaterial;

    [MenuItem("Tools/Assign Material to Selection")]
    static void Init()
    {
        GetWindow<AssignMaterial>("Assign Material");
    }

    void OnGUI()
    {
        newMaterial = (Material)EditorGUILayout.ObjectField("Material", newMaterial, typeof(Material), false);

        if (GUILayout.Button("Apply to Selected Objects"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Undo.RecordObject(renderer, "Change Material");
                    renderer.material = newMaterial;
                }
            }
        }
    }
}