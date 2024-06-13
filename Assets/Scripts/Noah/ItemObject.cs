using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour{
    [SerializeField] public InventoryItemData _treferenceItem;
    public InventoryItemDataWrapper referenceItem;
    [SerializeField] public float pickupBufferSeconds = 2f;

    public bool canPickUp = false;
    public CountdownTimer pickupCooldown;
    public ShopManager lastOwner;

    public void Awake(){
        Debug.Log("ITEM GOT SPANED");
        pickupCooldown = new CountdownTimer(pickupBufferSeconds);
        pickupCooldown.Start();
        pickupCooldown.OnTimerStop += () => { canPickUp = true;};
        referenceItem = new InventoryItemDataWrapper(_treferenceItem);
        //referenceItem.MakeNewWeapon();
    }

    void Update(){
        pickupCooldown.Tick(Time.deltaTime);
    }

   // public void SetData()
}
