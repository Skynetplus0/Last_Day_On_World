using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public float turnSpeed = 5f;
    public float reachThreshold = 0.1f;

    [Header("Offset")]
    public float spawnYOffset = 0f;

    [Header("Boss Settings")]
    public bool isBoss = false;
    public float bossSpeed = 0.55f;
    public float bossTurnSpeed = 12f;

    private Animator anim;
    private int currentIndex = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetFloat("SPEED", 0.5f);

        if (waypoints != null && waypoints.Length > 0)
        {
            Vector3 spawnPos = waypoints[0].position;
            spawnPos.y += spawnYOffset;
            transform.position = spawnPos;
            currentIndex = 1;
        }

        // Eğer boss ise ayrı hareket başlat
        if (isBoss)
        {
            StartCoroutine(BossMove());
        }
    }

    void Update()
    {
        // Normal zombiler normal hareketi Update içinde yapacak
        if (isBoss) return; // Boss kendi coroutine’inde hareket edecek

        if (waypoints == null || currentIndex >= waypoints.Length) return;

        Vector3 targetPos = waypoints[currentIndex].position;
        float fixedY = waypoints[0].position.y + spawnYOffset;
        targetPos.y = fixedY;

        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        Vector3 dirNorm = dir.normalized;

        if (anim != null)
            anim.SetFloat("SPEED", dir.magnitude);

        if (dirNorm != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirNorm);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }

        transform.position += dirNorm * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

        if (Vector3.Distance(transform.position, targetPos) < reachThreshold)
            currentIndex++;
    }

    // ===========================================
    // Boss için ayrı coroutine
    // ===========================================
    private IEnumerator BossMove()
    {
        currentIndex = 1; // Spawn pos sonrası
        while (currentIndex < waypoints.Length)
        {
            Vector3 targetPos = waypoints[currentIndex].position;
            float fixedY = waypoints[0].position.y + spawnYOffset;
            targetPos.y = fixedY;

            Vector3 dir = targetPos - transform.position;
            dir.y = 0;
            Vector3 dirNorm = dir.normalized;

            if (dirNorm != Vector3.zero)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirNorm);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, bossTurnSpeed * Time.deltaTime);
            }

            transform.position += dirNorm * bossSpeed * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

            if (Vector3.Distance(transform.position, targetPos) < reachThreshold)
                currentIndex++;

            yield return null;
        }

        // Boss yolu bitince destroy
        Destroy(gameObject);
    }
}

