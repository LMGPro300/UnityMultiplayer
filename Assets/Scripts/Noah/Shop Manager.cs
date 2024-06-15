using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;

/*
 * Program name: ShopManager.cs
 * Author: Noah Levy, slight help from tutorial
 * Local script that keeps track of the player balance and handles buying + updating UI
 */

//RELAVENT TUTORIAL FOR SHOP UI: https://www.youtube.com/watch?v=Oie-G5xuQNA

public class ShopManager : NetworkBehaviour
{ 
    //relavent UI references
    [SerializeField] private InventorySystem inventory;
    [SerializeField] public float playerCredits = 100;
    [SerializeField] private TextMeshProUGUI playerBalanceText;
    [SerializeField] private TextMeshProUGUI shopErrorText;
    [SerializeField] private TextMeshProUGUI canOpenShop;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private ShootingController shootingController;

    //flags for error cooldown and if shop UI is displaying
    private CountdownTimer timer = new CountdownTimer(2f);
    private bool canDisplayShop = false;
    public bool shopActive = false;

    //update UI on awake
    public void Awake(){
        UpdateBalance();
        timer.OnTimerStop += RemoveErrorMessage;
    }
    
    //tell player they can open shop when they approach
    public void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "shop"){
            canDisplayShop = true;
            canOpenShop.text = "Press G to Open Shop";
        }
    }

    //tell player they can leave shop and update UI
    public void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "shop"){
            canDisplayShop = false;
            canOpenShop.text = "";
            LeavingShop();
        }
    }

    //Recieve shop input from subscribed tabs
    public void RecieveShopInput(float shopInput){
        if (shopInput == 1f && canDisplayShop){
            shopUI.SetActive(!shopUI.activeSelf);
            shopActive = !shopActive;
            ChangeCursorState();
        }
    }

    //toggle cursor visibility
    private void ChangeCursorState(){
        if (Cursor.lockState == CursorLockMode.None){
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
        Cursor.visible = !Cursor.visible;
    }

    //logic for leaving the shop, toggling cursor visiblity
    public void LeavingShop(){
        if (shopActive){
            shopActive = false;
            shopUI.SetActive(!shopUI.activeSelf);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    //remember to update error cooldown timer
    void Update(){
        timer.Tick(Time.deltaTime);
    }

    //handles clicking items and displaying the poper error message
    //also subtracts from your credits
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

    //bellow update error messages for shop
    public void RemoveErrorMessage(){
        shopErrorText.text = "";
    }

    public void UpdateBalance(){
        playerBalanceText.text = "Credits: " + playerCredits;
    }
}
