using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputHandler : NetworkBehaviour//MonoBehaviour
{
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerShoot playerShoot;
    [SerializeField] PlayerPickUp playerPickUp;
    [SerializeField] InventorySystem inventorySystem;
    [SerializeField] PickUpItem pickUpItem;
    [SerializeField] ShopManager shopManager;
    //[SerializeField] PlayerPrediction clientPrediction;

    private PlayerControls PlayerControlsActionMap;
    private PlayerControls.KeyboardActions keyboardMovement;

    private Vector2 mouseInput;
    private Vector2 keyboardInput;
    private float jumpInput;
    private float shootInput;
    private float pickUpInput;
    private float inventoryInput;
    private float dropInput;
    private float reloadInput;
    private float shopInput;
    //private float teleportInput;

    void Awake()
    {
        PlayerControlsActionMap = new PlayerControls();
        keyboardMovement = PlayerControlsActionMap.Keyboard;
        keyboardMovement.JUMP.performed += ctx => jumpInput = ctx.ReadValue<float>();
        keyboardMovement.MOUSE.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();
        keyboardMovement.MOVEMENT.performed += ctx => keyboardInput = ctx.ReadValue<Vector2>();
        keyboardMovement.PICKUP.performed += ctx => pickUpInput = ctx.ReadValue<float>();
        keyboardMovement.INVENTORY.performed += ctx => inventoryInput = ctx.ReadValue<float>();
        keyboardMovement.DROP.performed += ctx => dropInput = ctx.ReadValue<float>();
        keyboardMovement.SHOOT.performed += ctx => shootInput = ctx.ReadValue<float>();
        keyboardMovement.RELOAD.performed += ctx => reloadInput = ctx.ReadValue<float>();
        keyboardMovement.SHOP.performed += ctx => shopInput = ctx.ReadValue<float>();
        //keyboardMovement.TELEPORT.performed += ctx => teleportInput = ctx.ReadValue<float>();

        //keyboardMovement.Primary.performed += ctx => shootInput = ctx.ReadValue<float>();
    }
    private void OnEnable(){
        keyboardMovement.Enable();
    }
    private void OnDisable(){
        keyboardMovement.Disable();
    }
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){ 
        if (!IsOwner) return;
        Debug.Log(!shopManager.shopActive + " shop input");
        if (inventoryInput == 0f && !shopManager.shopActive){
            playerCamera.RecieveInput(mouseInput);
        }
        playerMovement.RecieveKeyboardInput(keyboardInput);
        playerMovement.RecieveJumpInput(jumpInput);
        playerShoot.RecieveShootInput(shootInput);
        playerShoot.RecieveReloadInput(reloadInput);
        playerPickUp.RecievePickUpInput(pickUpInput);
        inventorySystem.RecieveInventoryInput(inventoryInput);
        inventorySystem.RecieveDropInput(dropInput);
        pickUpItem.RecievePickUpInput(pickUpInput);
        shopManager.RecieveShopInput(shopInput);
        shopInput = 0f;
        
        //clientPrediction.RecieveTeleportInput(teleportInput);
        //teleportInput = 0f;
        //playerShoot.RecieveShootInput(shootInput);      
    }
}