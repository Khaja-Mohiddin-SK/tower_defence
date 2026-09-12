using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Money;

    [Header("Starting Money")]
    public int startMoney = 650;

    void Start()
    {
        Money = startMoney;
    }
}