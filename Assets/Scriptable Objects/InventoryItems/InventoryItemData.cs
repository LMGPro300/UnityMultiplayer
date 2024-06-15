using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

/*
 * Program name: ShopItemData.cs
 * Author: Noah Levy
 * What the program does: houses static properties of shop items, allows for scalability
 */

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class InventoryItemData : ScriptableObject{
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public GameObject displayPrefab;
    public GameObject globalPrefab;
    public int maxStackSize = 1;
    public bool isGun;
    public bool isMelee;
    public GunScriptableObject refWeapon = null;
    public GunScriptableObject weapon = null;
    //public GunScriptableObjectWrapper realWeapon;
    public MeleeData melee = null;
    public string charAnimation = "Default|HoldItem";
    public string armAnimation = "Armature|PickUp";
    public float sellPrice = 10f;

    public void MakeNewWeapon(){
        if (refWeapon != null && weapon == null){
            weapon = Instantiate(refWeapon);
        }
    }
}


[System.Serializable]
public class InventoryItemDataWrapper{
    public InventoryItemData original;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public GameObject displayPrefab;
    public GameObject globalPrefab;
    public int maxStackSize = 1;
    public bool isGun;
    public bool isMelee;
    public GunScriptableObject refWeapon = null;
    public GunScriptableObjectWrapper weapon = null;
    public MeleeData melee = null;
    public string charAnimation = "Default|HoldItem";
    public string armAnimation = "Armature|PickUp";
    public float sellPrice = 10f;

    public InventoryItemDataWrapper(InventoryItemData inventoryItemData){
        original = inventoryItemData;
        displayName = inventoryItemData.displayName;
        icon = inventoryItemData.icon;
        prefab = inventoryItemData.prefab;
        displayPrefab = inventoryItemData.displayPrefab;
        globalPrefab = inventoryItemData.globalPrefab;
        maxStackSize = inventoryItemData.maxStackSize;
        isGun = inventoryItemData.isGun;
        isMelee = inventoryItemData.isMelee;
        refWeapon = inventoryItemData.refWeapon;
        weapon = refWeapon == null ? null : new GunScriptableObjectWrapper(inventoryItemData.refWeapon);
        //public GunScriptableObjectWrapper realWeapon;
        melee =  inventoryItemData.melee;
        charAnimation = inventoryItemData.charAnimation;
        armAnimation = inventoryItemData.armAnimation;
        sellPrice = inventoryItemData.sellPrice;
    }
}

