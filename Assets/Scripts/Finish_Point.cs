using UnityEngine;

public class Finish_Point : MonoBehaviour
{


    public EnemySpawnerNew spawner; // Inspector’dan baðla
    public int damageToBase = 1;

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Health.Instance.TakeDamage(damageToBase);

            if (spawner != null)
                spawner.onEnemyKilled();  


            Destroy(other.gameObject);
        }
    }
}