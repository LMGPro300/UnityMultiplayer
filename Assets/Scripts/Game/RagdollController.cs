using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: RagdollController.cs
 * Author: Elvin Shen
 * What the program does: Handles the ragdoll
 */

public class RagdollController : MonoBehaviour{
    private Rigidbody[] rigidbodies;


    //Get a list of the rigidbodies/body parts 
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

    //Usually called when the entity has died, enabled the ragdoll and find the closest
    //rigidbody the raycast hit, then apply a force to that rigidbody
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
