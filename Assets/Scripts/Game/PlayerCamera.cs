using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCamera : NetworkBehaviour//MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float sensX = 50.0f, sensY = -50.0f;
    private float camAngle = 0f;
    private Transform cam;

    public void Awake(){
        cam = _camera.transform;
    }

    public override void OnNetworkSpawn(){
        Debug.Log("spawned in");
        base.OnNetworkSpawn();
        if (!IsOwner) return; 
        _camera.enabled = true;
        //transform.rotation = Quaternion.Euler(-90f, 0f, 0f) * transform.rotation;
    }

    public void RecieveInput(Vector2 mouseInput){
        transform.rotation *= Quaternion.Euler(0f, mouseInput.x * Time.deltaTime * sensX, 0f);
        camAngle += mouseInput.y * sensY * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -90, 90);
        cam.rotation = transform.rotation * Quaternion.Euler(camAngle, 0f, 0f);
    }
}
