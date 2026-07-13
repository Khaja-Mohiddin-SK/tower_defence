using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Base Health")]
    public int protectTowerHealth = 20;

    void Awake()
    {
        Instance = this;
    }

    public void DamageProtectTower(int damage)
    {
        protectTowerHealth -= damage;

        Debug.Log("Protect Tower Health: " + protectTowerHealth);

        if (protectTowerHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("Game Over! Protect Tower destroyed.");
    }
}