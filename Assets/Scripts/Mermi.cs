using UnityEngine;

public class Mermi : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float destroyAfter = 3f;
    
    [Header("Target Settings")]
    [Tooltip("Hedefin yukarısına ne kadar yukseklik eklensin (boss icin 2-3)")]
    public float targetHeightOffset = 1.5f;
    
    private Transform target;
    private Vector3 lastKnownPosition;
    private bool hasTarget = false;
    private float detectedHeightOffset = 1.5f;

    public void Init(Transform targetTransform)
    {
        target = targetTransform;
        if (target != null)
        {
            // Boss mu kontrol et - daha yuksek nisan al
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null && enemy.isBoss)
            {
                detectedHeightOffset = 3f; // Boss icin daha yuksek
            }
            else
            {
                detectedHeightOffset = targetHeightOffset; // Normal zombi
            }
            
            UpdateTargetPosition();
            hasTarget = true;
            LookAtTarget();
        }
        
        Destroy(gameObject, destroyAfter);
    }
    
    void UpdateTargetPosition()
    {
        if (target != null)
        {
            // Hedefin pozisyonuna yukseklik ekle
            lastKnownPosition = target.position + Vector3.up * detectedHeightOffset;
        }
    }

    void Update()
    {
        if (!hasTarget) return;
        
        // Hedef hala varsa pozisyonunu guncelle
        if (target != null)
        {
            UpdateTargetPosition();
            LookAtTarget();
        }
        
        // Hedefe dogru hareket et
        transform.position = Vector3.MoveTowards(
            transform.position, 
            lastKnownPosition, 
            speed * Time.deltaTime
        );
        
        // Hedefe ulastiysa yok et
        if (Vector3.Distance(transform.position, lastKnownPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
    
    void LookAtTarget()
    {
        Vector3 direction = lastKnownPosition - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Dusmana carpinca yok ol
        if (other.CompareTag("Enemy") || other.GetComponent<Enemy>() != null)
        {
            Destroy(gameObject);
        }
    }
}
