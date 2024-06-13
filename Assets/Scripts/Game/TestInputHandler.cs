using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class TestInputHandler : NetworkBehaviour//MonoBehaviour
{
    //[SerializeField] PlayerCamera playerCamera;
    //[SerializeField] PlayerMovement playerMovement;
    //[SerializeField] PlayerShoot playerShoot;
    [SerializeField] TestMovement testMovement;
    [SerializeField] TestCamera testCamera;

    private PlayerControls PlayerControlsActionMap;
    private PlayerControls.KeyboardActions keyboardMovement;

    private Vector2 mouseInput;
    private Vector2 keyboardInput;
    private float jumpInput;
    private float shootInput;
    private float teleportInput;

    void Awake()
    {
        Debug.Log(IsOwner);
        PlayerControlsActionMap = new PlayerControls();
        keyboardMovement = PlayerControlsActionMap.Keyboard;
        //keyboardMovement.JUMP.performed += ctx => jumpInput = ctx.ReadValue<float>();
        keyboardMovement.MOUSE.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        keyboardMovement.MOVEMENT.performed += ctx => keyboardInput = ctx.ReadValue<Vector2>();
        keyboardMovement.TELEPORT.performed += ctx => teleportInput = ctx.ReadValue<float>();
        //keyboardMovement.Primary.performed += ctx => shootInput = ctx.ReadValue<float>();
    }
    private void OnEnable(){
        keyboardMovement.Enable();
    }
    private void OnDisable()
    {
        keyboardMovement.Disable();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    { 
        if (!IsOwner) return;
        //playerCamera.RecieveInput(mouseInput);
        //playerMovement.RecieveKeyboardInput(keyboardInput);
        testMovement.RecieveKeyboardInput(keyboardInput);
        testMovement.RecieveTeleportInput(teleportInput);
        testCamera.RecieveInput(mouseInput);
        teleportInput = 0f;
        //playerMovement.RecieveJumpInput(jumpInput);
        //playerShoot.RecieveShootInput(shootInput);      
    }
}