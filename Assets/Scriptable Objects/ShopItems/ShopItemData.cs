using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Shop Item")]
public class ShopItemData : ScriptableObject
{
    public int price;
    public Sprite icon;
    public string shopItemName;
    public int quantity;
    public InventoryItemData inventory_item;
}