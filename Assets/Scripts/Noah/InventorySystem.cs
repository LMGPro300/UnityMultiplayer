using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using Unity.Netcode;

[Serializable]
public class InventorySystem : NetworkBehaviour
{
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

    [SerializeField] Transform playerTransform;
    [SerializeField] Transform playerCamera;
    [SerializeField] PlayerShoot playerShoot;
    [SerializeField] MeleeManager playerMelee;
    [SerializeField] ItemSync itemSync;

    public Dictionary<InventoryItemData, List<InventoryItem>> item_dict;

    private int curSlot = 1;
    private bool inventoryIsDisplayed = false;

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
        Vector2 mousePos = getMouseCoords.ReadValue<Vector2>();
        float mouseAngle = fixAngleVal((Mathf.Atan2(-(mousePos.y - Screen.height / 2), mousePos.x - Screen.width / 2) * Mathf.Rad2Deg) - 270 + 360) % 360;
        if ((int)((mouseAngle) / 60f) + 1 != curSlot)
        {
            ChangeSlot((int)((mouseAngle) / 60f) + 1);
        }
    }

    private float fixAngleVal(float angle)
    {
        if (angle < 0){
            return 360 + angle;
        }
        return angle;
    }

    private void Awake(){
        inventory = new InventoryItem[6] {null, null, null, null, null, null};
        item_dict = new Dictionary<InventoryItemData, List<InventoryItem>>();
        numKeys.Enable();
        getMouseCoords.Enable();
        displayRadialMenu.Enable();
        numKeys.performed += ChangeSlot;
        UpdateHotbar();
    }

    private void OnDisable()
    {
        numKeys.Disable();
        displayRadialMenu.Disable();
        getMouseCoords.Disable();
    }

    public List<InventoryItem> Get(InventoryItemData referenceData) { 
        if (item_dict.TryGetValue(referenceData, out List<InventoryItem> value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        if (item_dict.TryGetValue(referenceData, out List<InventoryItem> value))
        {
            //find index in value list that we can add an item too
            int bestIndex = addToBestStack(value, referenceData.maxStackSize);

            //if there's a stack of items we can add to
            if (bestIndex != -1)
            {
                item_dict[referenceData][bestIndex].AddToStack();
            }
            //otherwise create a new stack
            else
            {
                InventoryItem newItem = new InventoryItem(referenceData);
                addToBestSlot(newItem);
                item_dict[referenceData].Add(newItem);
            }
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            addToBestSlot(newItem);
            item_dict[referenceData] = new List<InventoryItem> { newItem };
        }
        UpdateHotbar();
    }

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

    public void addToBestSlot(InventoryItem itemToAdd)
    {
        for (int i = 0; i <  inventory.Length; i++)
        {
            if (inventory[i] == null) {inventory[i] = itemToAdd; break;}
        }
    }

    public void Remove(InventoryItemData referenceData)
    {
        InventoryItem memoryAdressToRemovedItem = inventory[curSlot - 1];
        if (item_dict.TryGetValue(referenceData, out List<InventoryItem> value))
        {
            memoryAdressToRemovedItem.RemoveFromStack();
            if (memoryAdressToRemovedItem.stackSize == 0)
            {
                removeItemFromSlot(memoryAdressToRemovedItem);
                item_dict[referenceData].Remove(memoryAdressToRemovedItem);
            }
        }
        UpdateHotbar();
    }

    public void removeItemFromSlot(InventoryItem itemToRemove)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i] == itemToRemove) { inventory[i] = null; break; }
        }
    }

    public void RecieveDropInput(float dropInput){
        if (dropInput == 1f){
            DropItem();
        }
    }

    public void DropItem()
    {
        if (inventory[curSlot-1] == null)
        {
            return;
        }

        InventoryItem itemToDrop = inventory[curSlot-1];
        InventoryItemData referenceData = inventory[curSlot-1].data;
        GameObject itemToSpawn = itemToDrop.data.prefab;

        //GameObject droppedItem = Instantiate(itemToSpawn);
        SyncWithWorldSpace.Instance.InstantiateOnServer(itemToSpawn, (playerTransform.position + (playerCamera.forward * 2f)), new Quaternion(0f, 0f, 0f, 1), (Vector3.up * 4f) + (playerCamera.forward * 4f));
        //droppedItem.transform.position = playerTransform.position + (playerCamera.forward * 2f);
        Remove(referenceData);
    }

    public void PickUpItem(InventoryItemData referenceData, GameObject go){
        if (go == null) return;
        if (canAddItem(referenceData))
        {
            Add(referenceData);
            SyncWithWorldSpace.Instance.DestoryOnServer(go);
        }
    }

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

    public void UpdateHotbar(){
        Debug.Log(curSlot - 1);
        if (inventory[curSlot - 1] != null && inventory[curSlot - 1].data != null){
            pickUpAnimation.changeSlot(inventory[curSlot - 1].data.displayPrefab, inventory[curSlot - 1].data.animation);
            itemSync.GetNewObject(inventory[curSlot - 1].data.globalPrefab);
            playerShoot.ChangeSlot(inventory[curSlot - 1].data.weapon);
            playerMelee.ChangeSlot(inventory[curSlot - 1].data.melee);
            
        }
        else{
            playerShoot.ChangeSlot(null);
            playerMelee.ChangeSlot(null);
            pickUpAnimation.changeSlot(null, null);
            itemSync.Clear();
        }
        if (!hotbarChild.activeSelf) { return; }
        for (int i = 1; i <= 6; i++){
            if (inventory[i - 1] != null && inventory[i - 1].data == null){
                inventory[i - 1] = null;
            }
            GameObject mySlotObject = hotbarChild.transform.Find("Section " + i).gameObject;
            mySlotObject.GetComponent<Image>().color = (i != curSlot ? new Color32(125, 125, 125, 125) : new Color32(57, 57, 57, 125));
            mySlotObject.transform.Find("Image").GetComponent<Image>().sprite = inventory[i - 1] != null ? inventory[i - 1].data.icon : blankImage;
        }
        UpdateDisplayIcon();
    }

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

    public bool canAddItem(InventoryItemData itemToCheck)
    {
        if (inventory.Contains(null))
        {
            return true;
        }
        for (int i = 0; i < 6; i++)
        {
            if (inventory[i].data == itemToCheck && inventory[i].stackSize+1 <= itemToCheck.maxStackSize)
            {
                return true;
            }
        }
        return false;
    }
}
