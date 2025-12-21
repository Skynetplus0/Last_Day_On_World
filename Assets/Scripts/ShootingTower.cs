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

    [Header("Model Rotation")]
    public Transform model;      // ← assign ModelRoot here
    public float turnSpeed = 10f;

    Transform currentTarget;

    protected override void Update()
    {
        base.Update();

        if (currentTarget != null)
            RotateModelTowards(currentTarget);
    }

    protected override void OnTick()
    {
        currentTarget = FindNearestEnemy();
        if (currentTarget == null) return;

        Shoot(currentTarget);
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
        if (model == null) return;

        Vector3 dir = target.position - model.position;
        dir.y = 0f; // lock vertical rotation

        Quaternion targetRot = Quaternion.LookRotation(dir);
        model.rotation = Quaternion.Lerp(
            model.rotation,
            targetRot,
            Time.deltaTime * turnSpeed
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
