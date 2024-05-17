using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour{
    [SerializeField] private Transform cam;
    [SerializeField] private float sensX = 50.0f, sensY = -50.0f;
    private float camAngle = 0f;

    public void RecieveInput(Vector2 mouseInput){
        transform.rotation *= Quaternion.Euler(0f, mouseInput.x * Time.deltaTime * sensX, 0f);
        camAngle += mouseInput.y * sensY * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -90, 90);
        cam.rotation = transform.rotation * Quaternion.Euler(camAngle, 0f, 0f);
    }
}
