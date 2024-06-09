using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
        //Debug.Log("<color=green>landed ground</color>");
public class PlayerCollision : NetworkBehaviour//MonoBehaviour//
{
    [SerializeField] private int jumpFrameBuffer = 100;
    private bool isGrounded = false;
    private int currentJumpFrame;

    
    private void OnTriggerEnter(Collider other){
        string gotTag = other.gameObject.tag;
        if (gotTag == "ground"){
            currentJumpFrame = jumpFrameBuffer;
            isGrounded = true;
        }
    }

    private void OnTriggerStay(Collider other){
        string gotTag = other.gameObject.tag;
        if (gotTag == "ground"){
            if (currentJumpFrame != 0){
                currentJumpFrame -= 1;
            }
            isGrounded = true;
        } else if (gotTag == "Funny Slope"){
            isGrounded = false;
        }
    }

    private void OnTriggerExit(Collider other){
        string gotTag = other.gameObject.tag;
        if (gotTag == "ground"){
            isGrounded = false;
        }
    }

    public void DidJump(){
        isGrounded = false;
    }

    public bool GetIsGrounded(){
        return isGrounded;
    }

    public bool GetIsAir(){
        return isGrounded && currentJumpFrame == 0;
    }
}
