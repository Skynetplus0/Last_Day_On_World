using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level2EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveData
    {
        public int enemyCount;
        public float delayBetweenEnemies = 0.25f;
    }

    [Header("Prefabs / Nodes")]
    public GameObject enemyPrefab;
    public PathNode startNode;

    [Header("Waves")]
    public List<WaveData> waves = new List<WaveData>();

    [Header("Spawn")]
    public float spawnYOffset = 0f;

    [Header("Mode")]
    public bool deterministic = true;

    [Header("Wave UI")]
    public GameObject waveUIPanel;
    public TextMeshProUGUI waveCompletedText;
    public Button startWaveButton;
    
    [Header("Wave Progress")]
    public WaveProgressUI waveProgressUI;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int aliveEnemyCount = 0;

    private List<List<int>> plannedChoiceSequences;

    void Awake()
    {
        if (startWaveButton != null)
        {
            startWaveButton.onClick.RemoveAllListeners();
            startWaveButton.onClick.AddListener(StartNextWave);
        }
        Debug.Log("[Spawner] Awake. Waves count: " + waves.Count);
    }

    public void StartNextWave()
    {
        Debug.Log("[Spawner] StartNextWave called. isSpawning=" + isSpawning + " currentWaveIndex=" + currentWaveIndex);

        if (isSpawning)
        {
            Debug.Log("[Spawner] Already spawning, ignoring StartNextWave.");
            return;
        }

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("[Spawner] All waves completed. currentWaveIndex=" + currentWaveIndex);
            return;
        }

        // Disable start button while wave runs
        if (startWaveButton != null)
            startWaveButton.interactable = false;

        if (waveUIPanel != null)
            waveUIPanel.SetActive(false);

        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    IEnumerator SpawnWave(WaveData wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnOne(i);
            yield return new WaitForSeconds(wave.delayBetweenEnemies);
        }

        isSpawning = false;

        // 🔥 LEVEL 1 MANTIĞI
        yield return new WaitForSeconds(5f);
        WaveCompleted();
    }


    void SpawnOne(int index)
    {
        Vector3 pos = startNode.transform.position;
        pos.y += spawnYOffset;

        GameObject go = Instantiate(enemyPrefab, pos, Quaternion.identity);
        if (go == null)
        {
            Debug.LogError("[Spawner] Instantiate returned null!");
            return;
        }

        EnemyMoverNode mover = go.GetComponent<EnemyMoverNode>();
        if (mover == null)
        {
            Debug.LogError("[Spawner] Spawned prefab missing EnemyMoverNode!");
            Destroy(go);
            return;
        }

        mover.currentNode = startNode;

        if (deterministic && plannedChoiceSequences != null && index < plannedChoiceSequences.Count)
        {
            mover.plannedChoices = new Queue<int>(plannedChoiceSequences[index]);
        }

        // increment count and subscribe
        aliveEnemyCount++;
        mover.onEnemyFinished += OnEnemyFinished;

        Debug.Log($"[Spawner] Spawned enemy #{index}. aliveEnemyCount={aliveEnemyCount}");
    }

  
    // ENEMY CALLBACK
    void OnEnemyFinished()
    {
        aliveEnemyCount--;
        Debug.Log("[Spawner] OnEnemyFinished called. remaining = " + aliveEnemyCount + " isSpawning=" + isSpawning);

        if (aliveEnemyCount <= 0 && !isSpawning)
        {
            Debug.Log("[Spawner] No alive enemies and not spawning -> WaveCompleted()");
            WaveCompleted();
        }
    }

    void WaveCompleted()
    {
        Debug.Log($"[Spawner] WaveCompleted called for waveIndex={currentWaveIndex}");

        // Wave Progress UI güncelle
        if (waveProgressUI != null)
            waveProgressUI.OnWaveCompleted();

        // SAFELY update UI (check nulls)
        if (waveCompletedText != null)
            waveCompletedText.text = $"Wave {currentWaveIndex + 1} Completed!";
        else
            Debug.LogWarning("[Spawner] waveCompletedText is NULL!");

        if (waveUIPanel != null)
            waveUIPanel.SetActive(true);
        else
            Debug.LogWarning("[Spawner] waveUIPanel is NULL!");

        if (startWaveButton != null)
            startWaveButton.interactable = true;
        else
            Debug.LogWarning("[Spawner] startWaveButton is NULL!");

        // finally advance index
        currentWaveIndex++;
        Debug.Log("[Spawner] currentWaveIndex incremented -> " + currentWaveIndex);
    }
    
    List<List<int>> BuildPlannedSequences(PathNode root, int totalCount)
    {
        List<List<int>> tickets = new List<List<int>>(totalCount);
        for (int i = 0; i < totalCount; i++)
            tickets.Add(new List<int>());

        List<int> indices = new List<int>();
        for (int i = 0; i < totalCount; i++)
            indices.Add(i);

        Distribute(root, indices);
        return tickets;

        void Distribute(PathNode node, List<int> ticketIndices)
        {
            if (node == null || node.isEnd || node.nextNodes.Count == 0)
                return;

            int k = node.nextNodes.Count;
            float totalW = 0f;
            float[] weights = new float[k];

            for (int i = 0; i < k; i++)
            {
                weights[i] =
                    (node.childWeights != null && node.childWeights.Count == k)
                    ? Mathf.Max(0f, node.childWeights[i])
                    : 1f;
                totalW += weights[i];
            }

            if (totalW <= 0f)
            {
                for (int i = 0; i < k; i++) weights[i] = 1f;
                totalW = k;
            }

            int[] counts = new int[k];
            int assigned = 0;

            for (int i = 0; i < k; i++)
            {
                counts[i] =
                    Mathf.FloorToInt(weights[i] / totalW * ticketIndices.Count);
                assigned += counts[i];
            }

            int leftover = ticketIndices.Count - assigned;
            for (int i = 0; i < k && leftover > 0; i++)
            {
                counts[i]++;
                leftover--;
            }

            int cursor = 0;
            for (int i = 0; i < k; i++)
            {
                if (counts[i] <= 0) continue;

                var sub = ticketIndices.GetRange(cursor, counts[i]);
                cursor += counts[i];

                foreach (int idx in sub)
                    tickets[idx].Add(i);

                Distribute(node.nextNodes[i], sub);
            }
        }
    }
}
