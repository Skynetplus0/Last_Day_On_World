using UnityEngine;


public class Mermi : MonoBehaviour
{
    public float speed = 25f;
    private Transform target;

    public void Init(Transform enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = target.position + Vector3.up * 0.5f;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        transform.LookAt(targetPos);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.transform == target)
        {
            Destroy(gameObject); // SADECE GÖRÜNTÜ
        }
    }
}