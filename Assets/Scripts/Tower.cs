using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range = 5f;        // menzil
    public float fireRate = 1f;     // saniyede kaç atýþ
    public float damage = 20f;      // tek atýþta vereceði hasar
    public LayerMask enemyLayer;    // hangi layer'lar düþman

    private float fireCountdown = 0f;

    private void Update()
    {
        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            ShootAtNearestEnemy();
            fireCountdown = 1f / fireRate;
        }
    }

    void ShootAtNearestEnemy()
    {
        // Kule etrafýnda menzil kadar bir küre içinde düþman ara
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        if (hits.Length == 0)
            return;

        // En yakýndaki düþmaný bul (isteðe göre direkt hits[0] da kullanabilirsin)
        Collider nearest = null;
        float nearestDist = Mathf.Infinity;

        foreach (Collider c in hits)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = c;
            }
        }

        if (nearest != null)
        {
            Enemy enemy = nearest.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    // Editörde menzili görsel olarak çizmek için
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
