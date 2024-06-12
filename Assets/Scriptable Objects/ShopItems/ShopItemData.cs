using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Items/New Shop Item")]
public class ShopItemData : ScriptableObject
{
    public int price;
    public Sprite icon;
    public string shopItemName;
    public int quantity;
    public InventoryItemData inventory_item;
    public bool isAmmo;
    public AmmoData ammoData;
}