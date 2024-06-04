using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float playerHealth;
    public TextMeshProUGUI healthText;

    public void TakeDamage(float damageToTake)
    {
        playerHealth -= damageToTake;
    }

    public void GainHealth(float healthToGain)
    {
        playerHealth += healthToGain;
        healthText.text = playerHealth.ToString();
    }
}
