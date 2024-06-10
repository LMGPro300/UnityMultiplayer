using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    public InventoryItemData referenceItem;

    [SerializeField]
    public float pickupBufferSeconds = 2f;

    [SerializeField]
    public InventorySystem inventorySystem;

    public bool canPickUp = false;
    public CountdownTimer pickupCooldown;

    public DynamicItemData changingData;

    public void Awake()
    {
        //if (inventorySystem.GetDynamicData() == null)
        //{
        //    changingData = new DynamicItemData();
        //}
        //changingData = inventorySystem.GetDynamicData();
        pickupCooldown = new CountdownTimer(pickupBufferSeconds);
        pickupCooldown.Start();
        pickupCooldown.OnTimerStop += () => { canPickUp = true; Debug.Log("omg omg you can pick up the item now"); };
    }

    void Update()
    {
        pickupCooldown.Tick(Time.deltaTime);
    }
}
