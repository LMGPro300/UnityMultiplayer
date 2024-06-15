using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: PlayerCamera
 * Author: Elvin Shen
 * What the program does: Allows the player to look around via camera
 */

public class PlayerCamera : NetworkBehaviour//MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float sensX = 50.0f, sensY = -50.0f;
    private float camAngle = 0f;
    private Transform cam;
    [SerializeField] private float multiplier;
    [SerializeField] private float smooth;
    [SerializeField] private Transform armsModel;
    [SerializeField] private Transform sway;
    [SerializeField] private Transform head;
    [SerializeField] private MeshRenderer[] hiddenObjects;
    [SerializeField] private MeshRenderer[] showObjects;
    


    public void Awake(){
        cam = _camera.transform;
    }

    //Only allow the current player to turn on their own camera (rendering 2 cameras is weird)
    public override void OnNetworkSpawn(){
        Debug.Log("spawned in");
        base.OnNetworkSpawn();
        if (!IsOwner) return; 
        _camera.enabled = true;
        //some parts of the body are disabled on your side, but can be seen by other players, vice versa
        foreach (MeshRenderer part in hiddenObjects){
            part.enabled = false;
        }
        foreach (MeshRenderer part in showObjects){
            part.enabled = true;
        }

        //transform.rotation = Quaternion.Euler(-90f, 0f, 0f) * transform.rotation;
    }

    //Recieve the mouse input
    public void RecieveInput(Vector2 mouseInput){
        //Rotate the actual player along the y-axis (also moves the camera) and rotate the camera on the x axis
        transform.rotation *= Quaternion.Euler(0f, mouseInput.x * Time.deltaTime * sensX, 0f);
        camAngle += mouseInput.y * sensY * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -90, 90);
        cam.rotation = transform.rotation * Quaternion.Euler(camAngle, 0f, 0f);
        //Rotate
        armsModel.rotation = transform.rotation * Quaternion.Euler(camAngle, 0f, 0f);

   
        Quaternion rotationX = Quaternion.AngleAxis(-mouseInput.y * multiplier, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseInput.x * multiplier, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;
        sway.localRotation = Quaternion.Slerp(sway.localRotation, targetRotation, smooth * Time.deltaTime);
        //targetRotation = Mathf.Clamp(targetRotation.ang, -45, 45);
        //head.rotation = new Quaternion(cam.rotation.x, head.rotation.y, head.rotation.z, 1);
        float camAngle2 = Mathf.Clamp(camAngle, -45, 45);
        head.rotation = transform.rotation * Quaternion.Euler(camAngle2, 0f, 0f);//head.rotation * Quaternion.Euler(camAngle, 0f, 0f);
        head.rotation *= Quaternion.Euler(0f, 180f, 0f);
    }
    
}
