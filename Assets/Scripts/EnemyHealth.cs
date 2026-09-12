using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float startHealth = 100f;

    private float currentHealth;
    private bool isDead = false;

    [Header("Reward")]
    public int worth = 50;

    [Header("Death Effect")]
    public GameObject deathEffect;
    public float effectDestroyTime = 2f;

    void Start()
    {
        currentHealth = startHealth;
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
            return;

        currentHealth -= amount;

        Debug.Log(
            gameObject.name +
            " took " +
            amount +
            " damage. Health: " +
            currentHealth
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        // Reward player for destroying the enemy
        PlayerStats.Money += worth;

        if (deathEffect != null)
        {
            GameObject effect = Instantiate(
                deathEffect,
                transform.position,
                Quaternion.identity
            );

            Destroy(effect, effectDestroyTime);
        }

        Destroy(gameObject);
    }
}