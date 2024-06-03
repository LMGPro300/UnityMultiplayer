using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItem
{
    public ShopItemData data;
    public int stackSize = 0;
    public int quantity = 0;

    public ShopItem(ShopItemData shopItemData)
    {
        data = shopItemData;
        quantity = shopItemData.quantity;
        stackSize = quantity;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
        quantity--;
    }
}
