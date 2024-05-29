using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[Serializable]
public class InventorySystem : MonoBehaviour
{
    [SerializeField]
    public float pickupBufferSeconds = 2f;
    [SerializeField]
    public GameObject hotbarChild;
    [SerializeField]
    public InputAction numKeys;


    [SerializeField] PickUpAnimation pickUpAnimation;
    

    public Dictionary<InventoryItemData, InventoryItem> item_dict;
    public List<InventoryItem> inventory;
    //public List<GameObject> droppedItems;

    private int curSlot = 1;
    

    private void Awake()
    {
        inventory = new List<InventoryItem>();
        item_dict = new Dictionary<InventoryItemData, InventoryItem>();
        numKeys.Enable();
        numKeys.performed += ChangeSlot;
        UpdateHotbar();
    }

    private void OnDisable()
    {
        numKeys.Disable();
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
            Debug.Log("added to stack");
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            item_dict.Add(referenceData, newItem);
        }
        Debug.Log(inventory.Count);
        UpdateHotbar();
    }

    public void Remove(InventoryItemData referenceData)
    {
        if (item_dict.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();
            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                item_dict.Remove(referenceData);
            }
        }
        Debug.Log(inventory.Count);
        UpdateHotbar();
    }

    public void DropItem(Transform playerTransform, Transform cameraTransform)
    {
        if (curSlot > inventory.Count)
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
        Debug.Log("lkdfjslkasdfjl;kjadsfklj;adf");
        int pastSlot = curSlot;
        curSlot = int.Parse(ctx.control.name);
        Debug.Log(curSlot);
        UpdateHotbar();        
        //check if selected valid item
        //pickUpAnimation.changeSlot(inventory[curSlot-1].data.displayPrefab);
    }

    public void UpdateHotbar()
    {
        for (int i = 1; i <= 6; i++)
        {
            GameObject mySlotObject = hotbarChild.transform.Find("Slot " + i).gameObject;
            if (i <= inventory.Count)
            {
                mySlotObject.GetComponent<Image>().color = (i != curSlot ? Color.white : Color.green);
                mySlotObject.transform.Find("Image").GetComponent<Image>().sprite = inventory[i - 1].data.icon;
            }
            else
            {
                mySlotObject.GetComponent<Image>().color = (i != curSlot ? Color.white : Color.green);
                mySlotObject.transform.Find("Image").GetComponent<Image>().sprite = null;
            }
        }
        //pickUpAnimation.changeSlot(inventory[curSlot - 1].data.displayPrefab);
    }
}
