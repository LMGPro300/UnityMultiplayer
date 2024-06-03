using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFire : MonoBehaviour
{
    [SerializeField]
    public InputAction mouseClick;

    public void Start()
    {
        mouseClick.Enable();
        mouseClick.performed += mouseClicked;
    }
    public void OnDisable()
    {
        mouseClick.Disable();
    }

    public void mouseClicked(InputAction.CallbackContext ctx)
    {
        //Debug.Log("How will we finish this assignment in time");
    }
}
