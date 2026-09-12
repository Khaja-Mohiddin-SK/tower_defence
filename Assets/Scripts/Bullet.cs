using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;

    public float speed = 70f;

    [Header("Damage")]
    public float damage = 50f;

    [Header("Impact Effect")]
    public GameObject impactEffect;
    public float effectDestroyTime = 2f;

    public void Seek(Transform _target)
    {
        target = _target;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir =
            target.position - transform.position;

        float distanceThisFrame =
            speed * Time.deltaTime;

        if (dir.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(
            dir.normalized * distanceThisFrame,
            Space.World
        );

        transform.LookAt(target);
    }

    void HitTarget()
    {
        if (impactEffect != null)
        {
            GameObject effectInstance = Instantiate(
                impactEffect,
                transform.position,
                transform.rotation
            );

            Destroy(
                effectInstance,
                effectDestroyTime
            );
        }

        EnemyHealth enemyHealth =
            target.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}