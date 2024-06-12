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
        Debug.Log("PLAYER HEALTH IS NOW " + playerHealth);
        //healthText.text = playerHealth.ToString();
    }

    public void GainHealth(float healthToGain)
    {
        playerHealth += healthToGain;
        Debug.Log("PLAYER HEALTH IS NOW " + playerHealth);
        //healthText.text = playerHealth.ToString();
    }
}
