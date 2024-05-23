using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TestMovement : NetworkBehaviour
{
    private Vector3 wishDirection = Vector3.zero;
    [SerializeField] private Rigidbody rb;


    void Start()
    {
        
    }

    void Update(){
        if (!IsOwner) return;
        rb.velocity = wishDirection;

    }

    public void RecieveKeyboardInput(Vector2 keyboardInput){
        wishDirection.x = keyboardInput.x;
        wishDirection.z = keyboardInput.y;
        wishDirection = transform.TransformDirection(wishDirection);
    }
}
