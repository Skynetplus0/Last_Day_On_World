using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using UnityEditor;

public class EnemySpawnerNew : MonoBehaviour
{
    private float currentInterval;
    private float timer;
    private int currentWave = -1;
    private bool isSpawning = false;
    private int aliveEnemies = 0;

    [Header("Wave UI")]
    public GameObject waveUIPanel;
    public TextMeshProUGUI waveCompletedText;
    public Button startWaveButton;
    [Header("Wave Waypoints")]
    public Transform[] pathWaypoints;
    [Header("Waves")]
    public List<Wave> waves = new List<Wave>();

    [System.Serializable] //bu görünebilmesi için inspectorda
    public class Wave
    {
        public List<Enemy> enemies = new List<Enemy>();
        [System.Serializable]
        public class Enemy
        {
            public GameObject enemyPrefab;
            public int count;

        }
    }

    public void startNextWave()
    {
        if(isSpawning) return;
        currentWave++;
        if(currentWave >= waves.Count) return;
        waveUIPanel.SetActive(false);
        StartCoroutine(spawnWaveRoutine(waves[currentWave]));
    }

    IEnumerator spawnWaveRoutine(Wave wave)
    {
        isSpawning = true;
        foreach(var enemyType in wave.enemies)
        {
            if(enemyType.enemyPrefab == null) continue;
            for(int i = 0; i < enemyType.count; i++)
            {
                spawnEnemies(enemyType.enemyPrefab);
                yield return new WaitForSeconds(0.3f);
            }
        }
        isSpawning = false;
    }

    void spawnEnemies(GameObject enemyPrefab)
    {
        Vector3 spawnPos = pathWaypoints[0].position;
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        aliveEnemies++;
        EnemyMover mover = enemy.GetComponent<EnemyMover>();
        if(mover != null) mover.waypoints = pathWaypoints;
    }

    public void onEnemyKilled()
    {
        aliveEnemies--;
        if(aliveEnemies <= 0 && !isSpawning) waveCompleted();
    }

    void waveCompleted()
    {
        waveCompletedText.text = $"Wave {currentWave+1} Completed!";
        waveUIPanel.SetActive(true);
    }
}
