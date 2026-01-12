using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.SceneManagement;


public class EnemySpawnerNew : MonoBehaviour
{
    private float currentInterval;
    private float timer;
    private int currentWave = -1;
    private bool isSpawning = false;
    private int aliveEnemies = 0;

    [Header("Level Progression")]
    public float nextLevelDelay = 2f; // paneli gösterdikten kaç sn sonra geçsin



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
        
        // Wave baslangic sesi cal (muzik devam etsin)
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayWaveStart();
        
        // Zombiler gelmeden once kisa bekleme (gerilim)
        yield return new WaitForSeconds(2f);
        
        foreach(var enemyType in wave.enemies)
        {
            if(enemyType.enemyPrefab == null) continue;
            
            bool isBoss = enemyType.enemyPrefab.name.ToLower().Contains("boss");
            
            // Her enemy tipi icin 1 KEZ ses cal (ilk zombide)
            if (SoundManager.Instance != null)
            {
                if (isBoss)
                    SoundManager.Instance.PlayBossZombieSpawn();
                else
                    SoundManager.Instance.PlayZombieSpawn();
            }
            
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
        if (aliveEnemies <= 0 && !isSpawning)
        {
         
            waveCompleted();

        }
    }

    IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(nextLevelDelay);

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("Son level bitti! Build Settings'te sonraki sahne yok.");
        }
    }





    void waveCompleted()
    {
        waveCompletedText.text = $"Wave {currentWave + 1} Completed!";
        
        // SON WAVE BITTIYSE -> Level Complete paneli goster
        bool lastWave = (currentWave >= waves.Count - 1);
        if (lastWave)
        {
            // Level Complete UI goster
            if (LevelCompleteUI.Instance != null)
            {
                int score = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
                int coins = CoinManager.Instance != null ? CoinManager.Instance.coins : 0;
                int wavesCompleted = currentWave + 1;
                
                string levelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.ToUpper();
                LevelCompleteUI.Instance.ShowLevelComplete(levelName, score, coins, wavesCompleted);
            }
            else
            {
                // Fallback - eski davranis
                StartCoroutine(LoadNextSceneAfterDelay());
            }
        }
        else
        {
            waveUIPanel.SetActive(true);
        }
    }
}
