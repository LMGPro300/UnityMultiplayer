using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputHandler : NetworkBehaviour//MonoBehaviour
{
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerMovement playerMovement;
    //[SerializeField] PlayerShoot playerShoot;
    [SerializeField] PlayerPickUp playerPickUp;
    //[SerializeField] PlayerPrediction clientPrediction;

    private PlayerControls PlayerControlsActionMap;
    private PlayerControls.KeyboardActions keyboardMovement;

    private Vector2 mouseInput;
    private Vector2 keyboardInput;
    private float jumpInput;
    private float shootInput;
    private float pickUpInput;
    //private float teleportInput;

    void Awake()
    {
        PlayerControlsActionMap = new PlayerControls();
        keyboardMovement = PlayerControlsActionMap.Keyboard;
        keyboardMovement.JUMP.performed += ctx => jumpInput = ctx.ReadValue<float>();
        keyboardMovement.MOUSE.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        keyboardMovement.MOVEMENT.performed += ctx => keyboardInput = ctx.ReadValue<Vector2>();
        keyboardMovement.PICKUP.performed += ctx => pickUpInput = ctx.ReadValue<float>();
        //keyboardMovement.TELEPORT.performed += ctx => teleportInput = ctx.ReadValue<float>();

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
        playerCamera.RecieveInput(mouseInput);
        playerMovement.RecieveKeyboardInput(keyboardInput);
        playerMovement.RecieveJumpInput(jumpInput);
        playerPickUp.RecievePickUpInput(pickUpInput);
        //clientPrediction.RecieveTeleportInput(teleportInput);
        //teleportInput = 0f;
        //playerShoot.RecieveShootInput(shootInput);      
    }
}