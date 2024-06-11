using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour{
    [SerializeField] public InventoryItemData referenceItem;
    [SerializeField] public float pickupBufferSeconds = 2f;

    public bool canPickUp = false;
    public CountdownTimer pickupCooldown;

    public void Awake(){
        pickupCooldown = new CountdownTimer(pickupBufferSeconds);
        pickupCooldown.Start();
        pickupCooldown.OnTimerStop += () => { canPickUp = true;};
        //referenceItem.MakeWeaponCopy();
    }

    void Update(){
        pickupCooldown.Tick(Time.deltaTime);
    }

   // public void SetData()
}
