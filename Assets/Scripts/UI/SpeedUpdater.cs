using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedUpdater : MonoBehaviour{
    public TMP_Text Box;
    [SerializeField] private Rigidbody rb;


    void Update(){
        float velo = Mathf.Sqrt(rb.velocity.x * rb.velocity.x + rb.velocity.z * rb.velocity.z);
        Box.text = "Speed: " + velo;
    }
}
