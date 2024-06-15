using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
/*
 * Program name: SpeedUpdated.cs
 * Author: Elvin Shen 
 * What the program does: Used by VelocityVisualizer, displays text on the speed of the player
 */

public class SpeedUpdater : MonoBehaviour{
    public TMP_Text Box;
    [SerializeField] private Rigidbody rb;


    void Update(){
        float velo = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
        Box.text = "Speed: " + velo;
    }
}
