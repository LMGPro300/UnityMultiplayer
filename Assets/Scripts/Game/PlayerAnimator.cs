using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: PlayerAnimator.cs
 * Author: Elvin Shen
 * What the program does: Responsible for the animator other players see you preform
 */

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] PlayerCollision playerCollision;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] private Animator animator;

    //if the player jumps, play the jump animation, if it walks, play that animation
    void Update(){
        if (!IsOwner) return;
        Debug.Log("Is moving + " + playerMovement.IsMoving());
        Debug.Log("Is jump + " + !playerCollision.GetIsGrounded());
        animator.SetBool("Is_Walking", playerMovement.IsMoving());
        animator.SetBool("Is_Jumping", !playerCollision.GetIsGrounded());
    }
}
