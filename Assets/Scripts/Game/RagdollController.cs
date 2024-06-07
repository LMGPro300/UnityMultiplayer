using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour{
    private Rigidbody[] rigidbodies;

    void Awake(){
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();
    }
    public void DisableRagdoll(){
        foreach(Rigidbody rb in rigidbodies){
            rb.isKinematic = true;
        }
    }

    public void EnableRagdoll(){
        foreach(Rigidbody rb in rigidbodies){
            rb.isKinematic = false;
        }
    }

    public void TriggerRagdoll(Vector3 force, Vector3 hitLocation){
        EnableRagdoll();

        Rigidbody closestRigidbody = null;
        float closestDistance = 10000;
        
        foreach(Rigidbody rb in rigidbodies){
            float distance = Vector3.Distance(rb.position, hitLocation);
            if(distance < closestDistance){
                closestDistance = distance;
                closestRigidbody = rb;
            }
        } 
        closestRigidbody.AddForceAtPosition(force, hitLocation, ForceMode.Impulse);
    }
}
