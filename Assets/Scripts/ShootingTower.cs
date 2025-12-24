using UnityEngine;

public class ShootingTower : TowerBase
{
    [Header("Combat")]
    public float range = 5f;
    public float damage = 20f;
    public LayerMask enemyLayer;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f; // saniyede 2 mermi
    float fireTimer;

    [Header("Model Rotation")]
    public Transform model;      // ← assign ModelRoot here
    public float turnSpeed = 10f;

    Transform currentTarget;

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
            fireTimer = 0f; // istersen hedef yokken sıfırla
        }
    }
    protected override void OnTick()
    {
        /*
        currentTarget = FindNearestEnemy();
        if (currentTarget == null) return;

        Shoot(currentTarget);
    */
        }

    Transform FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);
        if (hits.Length == 0) return null;

        Transform nearest = null;
        float nearestDist = Mathf.Infinity;

        foreach (Collider c in hits)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = c.transform;
            }
        }

        return nearest;
    }

    void RotateModelTowards(Transform target)
    {
        if (model == null || target == null) return;

        Vector3 dir = target.position - model.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        // turnSpeed = derece/saniye gibi düşün
        model.rotation = Quaternion.RotateTowards(
            model.rotation,
            targetRot,
            turnSpeed * Time.deltaTime
        );
    }
    void Shoot(Transform target)
    {
        GameObject p = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

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
