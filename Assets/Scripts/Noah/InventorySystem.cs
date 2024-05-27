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
    

    public Dictionary<InventoryItemData, InventoryItem> item_dict;
    public List<InventoryItem> inventory;
    public List<GameObject> droppedItems;

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
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            item_dict.Add(referenceData, newItem);
        }
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
    }

    public void DropItem(Transform playerTransform, Transform cameraTransform)
    {
        Debug.Log("we made it here");
        InventoryItem itemToDrop = inventory[0];
        InventoryItemData referenceData = inventory[0].data;
        Debug.Log("akjdsfalskdjf");
        GameObject itemToSpawn = droppedItems[0];

        if (itemToDrop == null)
        {
            return;
        }

        GameObject droppedItem = Instantiate(itemToSpawn);
        droppedItem.transform.position = playerTransform.position + (cameraTransform.forward * 2f);
        droppedItem.GetComponent<Rigidbody>().AddForce((Vector3.up * 4f) + (cameraTransform.forward * 4f), ForceMode.Impulse);
    }

    public void PickUpItem(InventoryItemData referenceData, GameObject go)
    {
        Add(referenceData);
        Destroy(go);
    }

    public void ChangeSlot(InputAction.CallbackContext ctx)
    {
        curSlot = int.Parse(ctx.control.name);
        UpdateHotbar();
    }

    public void UpdateHotbar()
    {
        for (int i = 1; i <= inventory.Count; i++)
        {
            Debug.Log("alskdjflaksdjflkasjdflkjasdf");
            GameObject mySlotObject = hotbarChild.transform.Find("Slot " + i).gameObject;
            mySlotObject.GetComponent<Image>().color = (i != curSlot ? Color.white : Color.green);
            mySlotObject.transform.Find("Image").GetComponent<Image>().sprite = inventory[i-1].data.icon;
        }
    }
}
