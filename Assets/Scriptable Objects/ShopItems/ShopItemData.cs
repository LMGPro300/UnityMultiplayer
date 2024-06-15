using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * Program name: ShopItemData.cs
 * Author: Noah Levy
 * What the program does: houses static properties of shop items, allows for scalability
 */

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Items/New Shop Item")]
public class ShopItemData : ScriptableObject
{
    //simple variables, can be modified in the editor
    public int price;
    public Sprite icon;
    public string shopItemName;
    public int quantity;
    public InventoryItemData inventory_item;
    public bool isAmmo;
    public AmmoData ammoData;
}