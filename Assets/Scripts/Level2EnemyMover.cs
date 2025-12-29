using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyMoverNode : MonoBehaviour
{
    public PathNode currentNode;
    public float speed = 3f;
    public float reachThreshold = 0.2f;
    public float spawnYOffset = 0f;

    // Spawner tarafından set edilir
    [HideInInspector] public Queue<int> plannedChoices = null;

    // Spawner callback
    public System.Action onEnemyFinished;

    private bool isFinished = false; // güvenlik

    private void Start()
    {
        if (currentNode != null)
        {
            Vector3 spawnPos = currentNode.transform.position;
            spawnPos.y += spawnYOffset;
            transform.position = spawnPos;
        }
    }

    private void Update()
    {
        if (currentNode == null || isFinished) return;

        Vector3 target = currentNode.transform.position;
        target.y += spawnYOffset;

        Vector3 dir = target - transform.position;
        dir.y = 0;

        if (dir.magnitude > 0.01f)
        {
            transform.position += dir.normalized * speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir.normalized),
                10f * Time.deltaTime
            );
        }

        if (dir.magnitude < reachThreshold)
        {
            ChooseNextNode();
        }
    }

    void ChooseNextNode()
    {
        if (currentNode == null || isFinished) return;

        var childs = currentNode.nextNodes;

        // END NODE
        if (childs == null || childs.Count == 0 || currentNode.isEnd)
        {
            ReachEnd();
            return;
        }

        // Deterministic seçim
        if (plannedChoices != null && plannedChoices.Count > 0)
        {
            int idx = plannedChoices.Dequeue();
            idx = Mathf.Clamp(idx, 0, childs.Count - 1);
            currentNode = childs[idx];
            return;
        }

        if (currentNode.childWeights == null ||
            currentNode.childWeights.Count != childs.Count)
        {
            currentNode = childs[Random.Range(0, childs.Count)];
            return;
        }
        
        // Weighted random fallback
        float total = 0f;
        for (int i = 0; i < currentNode.childWeights.Count; i++)
            total += Mathf.Max(0f, currentNode.childWeights[i]);

        if (total <= 0f)
        {
            currentNode = childs[Random.Range(0, childs.Count)];
            return;
        }

        float r = Random.Range(0f, total);
        float acc = 0f;

        for (int i = 0; i < childs.Count; i++)
        {
            acc += Mathf.Max(0f, currentNode.childWeights[i]);
            if (r <= acc)
            {
                currentNode = childs[i];
                return;
            }
        }

        currentNode = childs[childs.Count - 1];
    }

    
    // DEATH HANDLING
    void ReachEnd()
    {
        // Eve ulaştı - hasar ver
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.ReachBase();
        }
        else
        {
            // Fallback: BaseHealth'e doğrudan hasar ver
            if (BaseHealth.Instance != null)
            {
                BaseHealth.Instance.TakeDamage(10);
            }
            FinishEnemy();
        }
    }

    // Enemy ölünce çağırılacak
    public void Die()
    {
        FinishEnemy();
    }

    void FinishEnemy()
    {
        if (isFinished) return;

        isFinished = true;

        onEnemyFinished?.Invoke();

        Destroy(gameObject);
    }
}
