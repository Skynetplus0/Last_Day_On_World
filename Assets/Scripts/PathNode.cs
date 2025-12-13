using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [Tooltip("Sıradaki node'lar (bağlantılar).")]
    public List<PathNode> nextNodes = new List<PathNode>();

    [Tooltip("Her child için ağırlık. Eğer boşsa eşit davranır.")]
    public List<float> childWeights = new List<float>();

    [Tooltip("Bu node bir end ise true yap.")]
    public bool isEnd = false;

    private void OnValidate()
    {
        // childWeights uzunluğunu nextNodes ile eşitle (editor kolaylığı)
        if (childWeights == null) childWeights = new List<float>();
        while (childWeights.Count < nextNodes.Count) childWeights.Add(1f);
        while (childWeights.Count > nextNodes.Count) childWeights.RemoveAt(childWeights.Count - 1);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isEnd ? Color.red : Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        Gizmos.color = Color.cyan;
        if (nextNodes != null)
        {
            foreach (var n in nextNodes)
            {
                if (n == null) continue;
                Gizmos.DrawLine(transform.position, n.transform.position);
            }
        }
    }
}