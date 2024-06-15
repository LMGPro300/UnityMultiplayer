using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;

public class ShopManager : NetworkBehaviour
{ 
    [SerializeField] private InventorySystem inventory;
    [SerializeField] public float playerCredits = 100;
    [SerializeField] private TextMeshProUGUI playerBalanceText;
    [SerializeField] private TextMeshProUGUI shopErrorText;
    [SerializeField] private TextMeshProUGUI canOpenShop;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private ShootingController shootingController;
    private CountdownTimer timer = new CountdownTimer(2f);
    private bool canDisplayShop = false;
    public bool shopActive = false;

    public void Awake(){
        UpdateBalance();
        timer.OnTimerStop += RemoveErrorMessage;
    }
    
    public void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "shop"){
            canDisplayShop = true;
            canOpenShop.text = "Press G to Open Shop";
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "shop"){
            canDisplayShop = false;
            canOpenShop.text = "";
            LeavingShop();
        }
    }

    public void RecieveShopInput(float shopInput){
        if (shopInput == 1f && canDisplayShop){
            shopUI.SetActive(!shopUI.activeSelf);
            shopActive = !shopActive;
            ChangeCursorState();
        }
    }

    private void ChangeCursorState(){
        if (Cursor.lockState == CursorLockMode.None){
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
        Cursor.visible = !Cursor.visible;
    }

    public void LeavingShop(){
        if (shopActive){
            shopActive = false;
            shopUI.SetActive(!shopUI.activeSelf);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update(){
        timer.Tick(Time.deltaTime);
    }

    public void itemClicked(ShopItemData clickedObject){
        if (clickedObject.inventory_item != null && !inventory.canAddItem(clickedObject.inventory_item)){
            timer.Start();
            shopErrorText.text = "Your inventory is full";
        }
        else if (playerCredits - clickedObject.price < 0)
        {
            timer.Start();
            shopErrorText.text = "You don't have enough credits";
        }
        else if (clickedObject.isAmmo)
        {
            AmmoData ammoData = clickedObject.ammoData;
            if (ammoData.type == "small")
            {
                shootingController.smallAmmo += ammoData.packSize;
            }
            else if (ammoData.type == "medium")
            {
                shootingController.mediumAmmo += ammoData.packSize;
            }
            else
            {
                shootingController.largeAmmo += ammoData.packSize;
            }
            playerCredits -= clickedObject.price;
            UpdateBalance();
        }
        else{
            playerCredits -= clickedObject.price;
            UpdateBalance();
            inventory.Add(new InventoryItemDataWrapper(clickedObject.inventory_item));
        }
    }

    public void RemoveErrorMessage(){
        shopErrorText.text = "";
    }

    public void UpdateBalance(){
        playerBalanceText.text = "Credits " + playerCredits;
    }
}
