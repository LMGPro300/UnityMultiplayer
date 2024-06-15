using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: TestCamera.cs
 * Author: Elvin Shen
 * What the program does: Very barebones version of PlayerCamera to test networking
 */

public class TestCamera : NetworkBehaviour{
    [SerializeField] private Camera _camera;
    [SerializeField] private float sensX = 50.0f, sensY = -50.0f;
    private float camAngle = 0f;
    private Transform cam;

    public void Awake(){
        cam = _camera.transform;
    }

    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!IsOwner) return; 
        _camera.enabled = true;
    }

    public void RecieveInput(Vector2 mouseInput){
        transform.rotation *= Quaternion.Euler(0f, mouseInput.x * Time.deltaTime * sensX, 0f);
        camAngle += mouseInput.y * sensY * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -90, 90);
        cam.rotation = transform.rotation * Quaternion.Euler(camAngle, 0f, 0f);
    }
}
