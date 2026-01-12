using UnityEngine;

public class ShootingTower : TowerBase
{
    [Header("Combat")]
    public float range = 15f;
    public float damage = 20f;
    public LayerMask enemyLayer;
    
    [Header("Alternative Enemy Detection")]
    [Tooltip("Eger enemyLayer calismiyorsa, Enemy tag'i ile arama yapar")]
    public bool useTagInsteadOfLayer = true;
    public string enemyTag = "Enemy";

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    float fireTimer;

    [Header("Model Rotation")]
    public Transform model;
    public float turnSpeed = 200f;

    Transform currentTarget;
    
    void Start()
    {
        // Fire point yoksa kendini kullan
        if (firePoint == null)
            firePoint = transform;
            
        // Model yoksa kendini kullan (tum kule doner)
        if (model == null)
            model = transform;
            
        Debug.Log($"[ShootingTower] {name} basladi. Range: {range}, Model: {model?.name}, FirePoint: {firePoint?.name}");
    }

    protected override void Update()
    {
        base.Update();

        currentTarget = FindNearestEnemy();

        if (currentTarget != null)
        {
            RotateModelTowards(currentTarget);

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f)
            {
                Shoot(currentTarget);
                fireTimer = 1f / fireRate;
            }
        }
        else
        {
            fireTimer = 0f;
        }
    }
    
    protected override void OnTick()
    {
        // Base class gerektiriyor, bos birakilabilir
    }

    Transform FindNearestEnemy()
    {
        Transform nearest = null;
        float nearestDist = range;
        
        if (useTagInsteadOfLayer)
        {
            // Tag ile arama (daha guvenilir)
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            
            foreach (GameObject enemy in enemies)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy.transform;
                }
            }
        }
        else
        {
            // Layer ile arama
            Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);
            
            foreach (Collider c in hits)
            {
                float dist = Vector3.Distance(transform.position, c.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = c.transform;
                }
            }
        }

        return nearest;
    }

    void RotateModelTowards(Transform target)
    {
        if (model == null || target == null) return;

        // Sadece yatay yone bak (Y ekseni etrafinda don)
        Vector3 targetPos = target.position;
        Vector3 modelPos = model.position;
        
        // Y degerlerini esitle - sadece yatay rotasyon
        targetPos.y = modelPos.y;
        
        Vector3 dir = targetPos - modelPos;
        if (dir.sqrMagnitude < 0.0001f) return;

        // Sadece Y ekseni etrafinda donen rotasyon
        Quaternion targetRot = Quaternion.LookRotation(dir);
        
        // Mevcut rotasyonu al ve sadece Y rotasyonunu degistir
        Vector3 currentEuler = model.eulerAngles;
        Vector3 targetEuler = targetRot.eulerAngles;
        
        // X ve Z rotasyonunu koru, sadece Y'yi degistir
        float newY = Mathf.MoveTowardsAngle(currentEuler.y, targetEuler.y, turnSpeed * Time.deltaTime);
        model.eulerAngles = new Vector3(currentEuler.x, newY, currentEuler.z);
    }
    
    void Shoot(Transform target)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"[ShootingTower] {name}: projectilePrefab atanmamis!");
            return;
        }
        
        if (firePoint == null)
        {
            Debug.LogWarning($"[ShootingTower] {name}: firePoint atanmamis!");
            return;
        }

        GameObject p = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        // Atis sesi cal
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayShoot();

        Mermi m = p.GetComponent<Mermi>();
        if (m != null)
            m.Init(target);

        Enemy e = target.GetComponent<Enemy>();
        if (e != null)
            e.TakeDamage(damage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
