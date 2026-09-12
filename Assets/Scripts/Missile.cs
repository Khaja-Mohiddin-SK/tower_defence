using UnityEngine;

public class Missile : MonoBehaviour
{
    private Transform target;

    public float speed = 8f;
    public float rotateSpeed = 200f;

    [Header("Damage")]
    public float damage = 75f;

    [Header("Impact")]
    public GameObject impactEffect;
    public float effectDestroyTime = 2f;

    public void Seek(Transform targetToSeek)
    {
        target = targetToSeek;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction =
            target.position - transform.position;

        float distanceThisFrame =
            speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        Quaternion lookRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation,
                lookRotation,
                rotateSpeed * Time.deltaTime
            );

        transform.Translate(
            Vector3.forward * distanceThisFrame
        );
    }

    void HitTarget()
    {
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(
                impactEffect,
                transform.position,
                transform.rotation
            );

            Destroy(
                effect,
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