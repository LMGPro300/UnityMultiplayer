using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour{
    [SerializeField] private int jumpFrameBuffer = 100;
    private bool isGrounded = false;
    private int currentJumpFrame;
    
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("<color=green>landed ground</color>");
        currentJumpFrame = jumpFrameBuffer;
        isGrounded = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (currentJumpFrame != 0){
            currentJumpFrame -= 1;
        }
        
        if (other.gameObject.tag == "Funny Slope"){
            isGrounded = false;
        } else{
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }


    public bool GetIsGrounded(){
        return isGrounded;
    }

    public bool GetIsAir(){
        return isGrounded && currentJumpFrame == 0;
    }
}
