using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Unity.Netcode;

/*
 * Program name: InventorySystem.cs
 * Author: Noah Levy
 * What the program does: class for keeping track of player inventory
 */

[Serializable]
public class InventorySystem : NetworkBehaviour
{
    //respective UI elements
    [SerializeField]
    public GameObject hotbarChild;
    [SerializeField]
    public InputAction numKeys;
    [SerializeField]
    public InputAction displayRadialMenu;
    [SerializeField]
    public InputAction getMouseCoords;
    [SerializeField]
    public GameObject displayIcon;
    [SerializeField]
    public Sprite blankImage;
    [SerializeField] 
    PickUpAnimation pickUpAnimation;
    [SerializeField]
    public InventoryItem[] inventory;

    //respective player network references
    [SerializeField] NetworkObject playerNetworkObject;
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform playerCamera;
    [SerializeField] PlayerShoot playerShoot;
    [SerializeField] MeleeManager meleeManager;
    [SerializeField] ItemSync itemSync;

    //my dictionary
    public Dictionary<InventoryItemData, List<InventoryItem>> item_dict;

    //stores which slot we are on
    private int curSlot = 1;
    private bool inventoryIsDisplayed = false;

    //handles updating radial mouse logic
    private void Update(){
        if (inventoryIsDisplayed){
            RadialMouseLogic();
        }
        Debug.Log(" ran by the owner possibly");
    }

    public void RecieveInventoryInput(float inventoryInput){
        inventoryIsDisplayed = inventoryInput == 1f ? true : false;
        //if displaying radial inventory
        if (inventoryIsDisplayed && !hotbarChild.activeSelf) 
        {
            hotbarChild.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!inventoryIsDisplayed && hotbarChild.activeSelf){
            hotbarChild.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void RadialMouseLogic()
    {
        //uses atan2 to get which segment of the circle our mouse is on
        Vector2 mousePos = getMouseCoords.ReadValue<Vector2>();
        float mouseAngle = fixAngleVal((Mathf.Atan2(-(mousePos.y - Screen.height / 2), mousePos.x - Screen.width / 2) * Mathf.Rad2Deg) - 270 + 360) % 360;
        if ((int)((mouseAngle) / 60f) + 1 != curSlot)
        {
            ChangeSlot((int)((mouseAngle) / 60f) + 1);
        }
    }

    //fixes negative angles from atan2
    private float fixAngleVal(float angle)
    {
        if (angle < 0){
            return 360 + angle;
        }
        return angle;
    }

    //handles spawning item into network space
    public override void OnNetworkSpawn(){
        if (!IsOwner) return;
        base.OnNetworkSpawn();
        inventory = new InventoryItem[6] {null, null, null, null, null, null};
        item_dict = new Dictionary<InventoryItemData, List<InventoryItem>>();
        numKeys.Enable();
        getMouseCoords.Enable();
        displayRadialMenu.Enable();
        numKeys.performed += ChangeSlot;
        UpdateHotbar();
    }
    
    //disable necessary input actions
    private void OnDisable()
    {
        numKeys.Disable();
        displayRadialMenu.Disable();
        getMouseCoords.Disable();
    }


    //return given value, helpful getter function
    public List<InventoryItem> Get(InventoryItemDataWrapper referenceData) { 
        if (item_dict.TryGetValue(referenceData.original, out List<InventoryItem> value))
        {
            return value;
        }
        return null;
    }

    //try to add an item
    public void Add(InventoryItemDataWrapper referenceData){
        //if item exists in dicitonary
        if (item_dict.TryGetValue(referenceData.original, out List<InventoryItem> value))
        {
            Debug.Log("it exists somewhere");
            //find index in value list that we can add an item too
            int bestIndex = addToBestStack(value, referenceData.maxStackSize);

            //if there's a stack of items we can add to
            if (bestIndex != -1)
            {
                item_dict[referenceData.original][bestIndex].AddToStack();
            }
            //otherwise create a new stack
            else
            {
                InventoryItem newItem = new InventoryItem(referenceData);
                addToBestSlot(newItem);
                item_dict[referenceData.original].Add(newItem);
            }
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            addToBestSlot(newItem);
            item_dict[referenceData.original] = new List<InventoryItem> { newItem };
        }
        UpdateHotbar();
    }

    //loops through inventory and adds item to best stack, returns an idex
    public int addToBestStack(List<InventoryItem> listOfPlayerStacks, int maxStackSize)
    {
        for (int i = 0; i < listOfPlayerStacks.Count; i++)
        {
            if (listOfPlayerStacks[i].stackSize+1 <= maxStackSize)
            {
                return i;
            }
        }
        return -1;
    } 

    //adds item to first slot that isn't null
    public void addToBestSlot(InventoryItem itemToAdd)
    {
        for (int i = 0; i <  inventory.Length; i++)
        {
            if (inventory[i] == null) {inventory[i] = itemToAdd; break;}
        }
    }

    //logic to remove an item, deleting it if its stack size becomes 0
    public void Remove(InventoryItemDataWrapper referenceData)
    {
        InventoryItem memoryAdressToRemovedItem = inventory[curSlot - 1];
        if (item_dict.TryGetValue(referenceData.original, out List<InventoryItem> value))
        {
            memoryAdressToRemovedItem.RemoveFromStack();
            if (memoryAdressToRemovedItem.stackSize == 0)
            {
                removeItemFromSlot(memoryAdressToRemovedItem);
                item_dict[referenceData.original].Remove(memoryAdressToRemovedItem);
            }
        }
        UpdateHotbar();
    }

    //handle removing item from UI
    public void removeItemFromSlot(InventoryItem itemToRemove)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == itemToRemove) { inventory[i] = null; break; }
        }
    }

    //drop item 
    public void RecieveDropInput(float dropInput){
        if (dropInput == 1f){
            DropItem();
        }
    }

    //handles dropping item
    public void DropItem()
    {
        if (inventory[curSlot-1] == null)
        {
            return;
        }
        //necessary references for dropping
        InventoryItem itemToDrop = inventory[curSlot-1];
        InventoryItemDataWrapper referenceData = inventory[curSlot-1].data;
        GameObject itemToSpawn = itemToDrop.data.prefab;
        GunScriptableObjectWrapper refWeapon = referenceData.weapon;
        GunPayload gunPayload = new GunPayload(){
            magSize = 0,
            currentMagSize = 0,
            damage = 0,
            firerate = 0,
            reloadTime = 0,
            force = 0,
            ammoType = 0
            };

        if (refWeapon != null){
            gunPayload = new GunPayload(){
            magSize = refWeapon.magSize,
            currentMagSize = refWeapon.currentMagSize,
            damage = refWeapon.damage,
            firerate = refWeapon.firerate,
            reloadTime = refWeapon.reloadTime,
            force = refWeapon.force,
            ammoType = refWeapon.ammoType
            };
        }
        Debug.Log("it's spawing");
        Debug.Log(playerNetworkObject);
        Debug.Log(itemToSpawn);
        Debug.Log(gunPayload);
        SyncWithWorldSpace.Instance.InstantiateItemOnServer(playerNetworkObject, itemToSpawn, gunPayload, (playerTransform.position + (playerCamera.forward * 2f)), new Quaternion(0f, 0f, 0f, 1), (Vector3.up * 4f) + (playerCamera.forward * 4f));
        
        //remove item from storage after dropping it
        Remove(referenceData);
    }

    //pick up item if you can, using a wrapper to ensure unique data
    public void PickUpItem(InventoryItemDataWrapper referenceData, GameObject go){
        if (go == null) return;
        if (canAddItem(referenceData))
        {
            Add(referenceData);
            SyncWithWorldSpace.Instance.DestoryOnServer(go);
        }
    }

    //overloaded function to change the hotbar slot
    public void ChangeSlot(InputAction.CallbackContext ctx)
    {
        if (!hotbarChild.activeSelf)
        {
            return;
        }
        curSlot = int.Parse(ctx.control.name);
        UpdateHotbar();
    }

    public void ChangeSlot(int newSlot)
    {
        if (!hotbarChild.activeSelf)
        {
            return;
        }
        curSlot = newSlot;
        UpdateHotbar();
    }

    //update the hotbar
    public void UpdateHotbar(){
        //add respective weapons where they need to go
        if (inventory[curSlot - 1] != null && inventory[curSlot - 1].data != null){
            pickUpAnimation.changeSlot(inventory[curSlot - 1].data.displayPrefab, inventory[curSlot - 1].data.charAnimation, inventory[curSlot- 1].data.armAnimation);
            itemSync.GetNewObject(inventory[curSlot - 1].data.globalPrefab);
            playerShoot.ChangeSlot(inventory[curSlot - 1].data.weapon);
            meleeManager.ChangeSlot(inventory[curSlot - 1].data.melee);
            
        }
        //you are holding nothing so don't do anything
        else{
            playerShoot.ChangeSlot(null);
            meleeManager.ChangeSlot(null);
            pickUpAnimation.changeSlot(null, null, null);
            itemSync.Clear();
        }
        if (!hotbarChild.activeSelf) { return; }
        //update UI accordingly
        for (int i = 1; i <= 6; i++){
            if (inventory[i - 1] != null && inventory[i - 1].data == null){
                inventory[i - 1] = null;
            }
            if (inventory[i - 1] != null && inventory[i - 1].data != null && inventory[i-1].data.original == null){
                inventory[i - 1] = null;
            }
            GameObject mySlotObject = hotbarChild.transform.Find("Section " + i).gameObject;
            mySlotObject.GetComponent<Image>().color = (i != curSlot ? new Color32(125, 125, 125, 125) : new Color32(57, 57, 57, 125));
            mySlotObject.transform.Find("Image").GetComponent<Image>().sprite = inventory[i - 1] != null ? inventory[i - 1].data.icon : blankImage;
        }
        UpdateDisplayIcon();
    }

    //updates radial menu logic
    public void UpdateDisplayIcon()
    {
        InventoryItem curItem = inventory[curSlot - 1];
        if (curItem == null)
        {
            displayIcon.transform.Find("Display").GetComponent<TextMeshProUGUI>().text = "";
            displayIcon.transform.Find("Item Count").GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            displayIcon.transform.Find("Display").GetComponent<TextMeshProUGUI>().text = ""+curItem.data.displayName;
            displayIcon.transform.Find("Item Count").GetComponent<TextMeshProUGUI>().text = ""+curItem.stackSize;
        }
    }

    //tests if can add item, overloaded functions
    public bool canAddItem(InventoryItemData itemToCheck)
    {
        if (inventory.Contains(null))
        {
            return true;
        }
        for (int i = 0; i < 6; i++)
        {
            if (inventory[i].data.displayName == itemToCheck.displayName && inventory[i].stackSize+1 <= itemToCheck.maxStackSize)
            {
                return true;
            }
        }
        return false;
    }

    public bool canAddItem(InventoryItemDataWrapper itemToCheck){
        if (inventory.Contains(null)){
            return true;
        }
        return false;

    }


    
}
