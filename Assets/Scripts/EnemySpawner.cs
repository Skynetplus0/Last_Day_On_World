using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] pathWaypoints;
    public GameObject bossPrefab;

    [Header("Timed Spawning")]
    public bool useTimedSpawn = false;
    public float startInterval = 3f;
    public float minInterval = 0.8f;
    public float difficultyRate = 0.95f;

    [Header("Manual Wave Spawning")]
    // useTimedSpawn kapalıysa bunu kullan
    public int number_of_enemy = 1;
    public bool autoSpawnOnStart = false;   // Oyunun başında otomatik dalga istiyorsan


    [Header("Wave UI")]
    public GameObject waveUIPanel;
    public TextMeshProUGUI waveCompletedText;
    public Button startWaveButton;

    private float currentInterval;
    private float timer;


    /*
    [Header("Wave Settings")]
    public float spawnDelayBetweenEnemies = 0.3f; // Aynı dalgadaki düşmanlar arası süre
    public int first_wave_number_of_enemy = 10;
    public int second_wave_number_of_enemy = 20;
    public int boss_wave_number_of_enemy = 30;

    private int currentWave = 0;
    private bool isSpawning = false;
    */
    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>();

    private int currentWave = -1;//-1 olmassa ilerden başlıyor
    private bool isSpawning = false;

    [System.Serializable] //bu görünebilmesi için inspectorda
    public class Wave
    {
        public int enemy_Count;
        public float delay_Between_Enemies;
        public bool hasBoss;
    }



    void Awake()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError($"[{name}] Awake aşamasında enemyPrefab NULL!", this);
        }
        else
        {
            Debug.Log($"[{name}] Awake -> enemyPrefab: {enemyPrefab.name}", this);
        }
    }
    void Start()
    {
        currentInterval = startInterval;

     /*
      //burası bi dursun 
        // Eğer zamanlı spawn kapalıysa ve otomatik dalga açıksa
        if (!useTimedSpawn && autoSpawnOnStart)
        {
            SpawnWave();   // number_of_enemy kadar zombi spawn eder
        }

    */
}

void Update()
    {

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartNextWave();
        }


        // Zamanlı spawn kapalıysa burası hiç çalışmaz, bu normal.
        if (!useTimedSpawn) return;

        timer += Time.deltaTime;

        if (timer >= currentInterval)
        {
            SpawnEnemy();
            timer = 0f;

            // Hızlanma
            currentInterval = Mathf.Max(minInterval, currentInterval * difficultyRate);
        }
    }



    public void StartNextWave()
    {
        waveUIPanel.SetActive(false); //yazı panel kapanıcak

        if (isSpawning)
        {
            Debug.Log("Zaten bir wave spawn ediliyor.");
            return;
        }

        currentWave++;

        //waveler bitince döner
        if (currentWave >= waves.Count)
        {
            //buraya ekleme gelicek waveler bitince bölüm bitti diye
            Debug.Log("Tüm wave ler tamamlandı");
            return;

        }
        // waves sınıfının içindeki waveleri nesneye atma işi burası
        Wave wave = waves[currentWave];

        StartCoroutine(SpawnWaveRoutine(wave.enemy_Count,wave.delay_Between_Enemies));

        /*  eski sistem
        switch (currentWave)
        {
            case 1:
               
                StartCoroutine(SpawnWaveRoutine(first_wave_number_of_enemy));
                break;

            case 2:
               
                StartCoroutine(SpawnWaveRoutine(second_wave_number_of_enemy));
                break;

            case 3:
               
                StartCoroutine(SpawnWaveRoutine(boss_wave_number_of_enemy));
                break;

            default:
                Debug.Log("Tüm wave'ler bitti!");
                break;
        }
    }
 
        */
    }






    // ============================================================
    // İstediğin anda belirli sayıda düşman göndermek için
    // (UI butonuna da bağlanabilir)
    // ============================================================
    
    /*
    public void SpawnWave()
    {
        StartCoroutine(SpawnWaveRoutine());
    }
    */
    private IEnumerator SpawnWaveRoutine(int count,float spawnDelayBetweenEnemies)
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab atanmadı!");
            yield break;
        }

        if (pathWaypoints == null || pathWaypoints.Length == 0)
        {
            Debug.LogError("Path waypoints atanmadı!");
            yield break;
        }

        isSpawning = true;


        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(); 
            yield return new WaitForSeconds(spawnDelayBetweenEnemies);
        }
        
        
 
         /*//BOSS İCİN, DÜZENLENECEK
        // 3. ve 4. wave’de boss spawn
        if ((currentWave == 2 || currentWave == 3) && bossPrefab != null)
        {
            GameObject boss = Instantiate(bossPrefab, pathWaypoints[0].position, Quaternion.identity);
            EnemyMover mover = boss.GetComponent<EnemyMover>();
            if (mover != null)
            {
                mover.waypoints = pathWaypoints;
                mover.isBoss = true;
                mover.spawnYOffset = 1.3f;
                mover.bossSpeed = 0.55f;
                mover.bossTurnSpeed = 12f;
            }

            Enemy enemyComp = boss.GetComponent<Enemy>();
            if (enemyComp != null)
                enemyComp.maxHealth = 1000f;
        }*/
        
        
        isSpawning = false;

        //Burada wave arası ekler Burası değişecek düşmanlar bitince yeni wave olması gerek
        yield return new WaitForSeconds(5.0f);

        // Wave bitti → UI göster
        waveCompletedText.text = $"Wave {currentWave} Completed!";
        waveUIPanel.SetActive(true);

    }
    // ============================================================
    // Tek düşman spawn kodu
    // ============================================================
    void SpawnEnemy()
    {
        if (enemyPrefab == null || pathWaypoints.Length == 0) return;

        Vector3 spawnPos = pathWaypoints[0].position;
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

        EnemyMover mover = enemy.GetComponent<EnemyMover>();
        if (mover != null)
        {
            mover.waypoints = pathWaypoints;
        }
    }
}