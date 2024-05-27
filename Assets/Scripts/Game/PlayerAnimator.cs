using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] PlayerCollision playerCollision;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] private Animator animator;


    void Update(){
        if (!IsOwner) return;
        Debug.Log("Is moving + " + playerMovement.IsMoving());
        Debug.Log("Is jump + " + !playerCollision.GetIsGrounded());
        animator.SetBool("Is_Walking", playerMovement.IsMoving());
        animator.SetBool("Is_Jumping", !playerCollision.GetIsGrounded());
    }
}
