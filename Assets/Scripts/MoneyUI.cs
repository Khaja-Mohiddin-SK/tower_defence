using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    void Update()
    {
        if (moneyText != null)
        {
            moneyText.text = "€" + PlayerStats.Money;
        }
    }
}