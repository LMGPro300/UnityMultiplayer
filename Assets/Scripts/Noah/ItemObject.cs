using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Program name: ItemObject.cs
 * Author: Noah Levy
 * What the program does: is present on each rendered item, handles spawning and picking up
 */

public class ItemObject : MonoBehaviour{

    //relavent wrapper and inspector references
    [SerializeField] public InventoryItemData _treferenceItem;
    public InventoryItemDataWrapper referenceItem;
    //puts a cooldown when the item spawns so you can't instantly pick it up after spawning it 
    //and glitching out the inventory system
    [SerializeField] public float pickupBufferSeconds = 2f;

    //flags and timers
    public bool canPickUp = false;
    public CountdownTimer pickupCooldown;
    public ShopManager lastOwner;

    public void Awake(){
        Debug.Log("ITEM GOT SPANED");
        //start respective timers, subscribing to functions when they finish
        pickupCooldown = new CountdownTimer(pickupBufferSeconds);
        pickupCooldown.Start();
        pickupCooldown.OnTimerStop += () => { canPickUp = true;};
        referenceItem = new InventoryItemDataWrapper(_treferenceItem);
        //referenceItem.MakeNewWeapon();
    }

    //tick cooldown timer when object spawns
    void Update(){
        pickupCooldown.Tick(Time.deltaTime);
    }

   // public void SetData()
}
