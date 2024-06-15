using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Program name: PlayerHealth.cs
 * Author: Noah Levy
 * What the program does: handles taking/gaining player damage and updating UI accordingly
 */

public class PlayerHealth : MonoBehaviour
{
    //relavent UI references
    public float playerHealth;
    public TextMeshProUGUI healthText;

    //allows to take and gain health, updating UI accordingly
    public void TakeDamage(float damageToTake)
    {
        playerHealth -= damageToTake;
        Debug.Log("PLAYER HEALTH IS NOW " + playerHealth);
        healthText.text = playerHealth.ToString();
    }

    public void GainHealth(float healthToGain)
    {
        playerHealth += healthToGain;
        Debug.Log("PLAYER HEALTH IS NOW " + playerHealth);
        healthText.text = playerHealth.ToString();
    }
}
