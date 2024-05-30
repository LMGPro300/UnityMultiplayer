using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[Serializable]
public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    public GameObject hotbarChild;
    [SerializeField]
    public InputAction numKeys;
    [SerializeField]
    public InputAction displayRadialMenu;
    [SerializeField]
    public InputAction getMouseCoords;

    [SerializeField] PickUpAnimation pickUpAnimation;
    

    public Dictionary<InventoryItemData, InventoryItem> item_dict;
    public InventoryItem[] inventory;
    public RectTransform canvas;

    private int curSlot = 1;
    private bool inventoryIsDisplayed = false;

    private void Update()
    {
        inventoryIsDisplayed = displayRadialMenu.inProgress;
        //if displaying radial inventory
        if (inventoryIsDisplayed && !hotbarChild.activeInHierarchy) 
        {
            hotbarChild.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UpdateHotbar();
        }
        else if (!inventoryIsDisplayed && hotbarChild.activeInHierarchy)
        {
            hotbarChild.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (inventoryIsDisplayed)
        {
            RadialMouseLogic();
        }
    }

    private void RadialMouseLogic()
    {
        Debug.Log(canvas.rect.width / 2);
        Vector2 mousePos = getMouseCoords.ReadValue<Vector2>();
        float mouseAngle = fixAngleVal(Mathf.Atan2(mousePos.y-Screen.height/2, mousePos.x-Screen.width/2) * Mathf.Rad2Deg);
        Debug.Log(mouseAngle + " " + (int)((mouseAngle + (90f - mouseAngle)) / 60f));
        ChangeSlot((int)((mouseAngle+(90-mouseAngle) / 60)));
    }

    private float fixAngleVal(float angle)
    {
        if (angle < 0)
        {
            return 360 + angle;
        }
        return angle;
    }

    private void Awake()
    {
        inventory = new InventoryItem[6];
        item_dict = new Dictionary<InventoryItemData, InventoryItem>();
        numKeys.Enable();
        getMouseCoords.Enable();
        displayRadialMenu.Enable();
        numKeys.performed += ChangeSlot;
        canvas = GetComponent<RectTransform>();
        UpdateHotbar();
    }

    private void OnDisable()
    {
        numKeys.Disable();
        displayRadialMenu.Disable();
        getMouseCoords.Disable();
    }

    public InventoryItem Get(InventoryItemData referenceData) { 
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            addToBestSlot(newItem);
            item_dict.Add(referenceData, newItem);
        }
        UpdateHotbar();
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
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                removeItemFromSlot(value);
                item_dict.Remove(referenceData);
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

    public void DropItem(Transform playerTransform, Transform cameraTransform)
    {
        if (inventory[curSlot-1] == null)
        {
            return;
        }

        InventoryItem itemToDrop = inventory[curSlot-1];
        InventoryItemData referenceData = inventory[curSlot-1].data;
        GameObject itemToSpawn = itemToDrop.data.prefab;

        if (itemToDrop == null)
        {
            return;
        }

        GameObject droppedItem = Instantiate(itemToSpawn);
        droppedItem.transform.position = playerTransform.position + (cameraTransform.forward * 2f);
        droppedItem.GetComponent<Rigidbody>().AddForce((Vector3.up * 4f) + (cameraTransform.forward * 4f), ForceMode.Impulse);
        Remove(referenceData);
        UpdateHotbar();
    }

    public void PickUpItem(InventoryItemData referenceData, GameObject go)
    {
        Add(referenceData);
        Destroy(go);
    }

    public void ChangeSlot(InputAction.CallbackContext ctx)
    {
        if (!inventoryIsDisplayed)
        {
            return;
        }
        curSlot = int.Parse(ctx.control.name);
        UpdateHotbar();
    }

    public void ChangeSlot(int newSlot)
    {
        if (!inventoryIsDisplayed)
        {
            return;
        }
        curSlot = newSlot;
        UpdateHotbar();
    }

    public void UpdateHotbar()
    {
        //pass in a display item only if selecting an item already
        if (inventory[curSlot - 1] != null)
        {
            pickUpAnimation.changeSlot(inventory[curSlot - 1].data.displayPrefab);
        }
        else
        {
            pickUpAnimation.changeSlot(null);
        }
        if (!hotbarChild.activeSelf) { return; }
        for (int i = 1; i <= 6; i++)
        {
            GameObject mySlotObject = hotbarChild.transform.Find("Section " + i).gameObject;
            mySlotObject.GetComponent<Image>().color = (i != curSlot ? Color.white : Color.green);
            mySlotObject.transform.Find("Image").GetComponent<Image>().sprite = inventory[i - 1] != null ? inventory[i - 1].data.icon : null;
        }
    }
}
