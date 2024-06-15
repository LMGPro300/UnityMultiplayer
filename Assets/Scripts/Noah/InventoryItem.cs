using System;

/*
 * Program name: ShopObject.cs
 * Author: Noah Levy, some help from tutorial
 * What the program does: wrapper class for each inventory data, holds unique data that scriptable
 * objects were not designed for
 */

//TUTORIAL USED: https://www.youtube.com/watch?v=SGz3sbZkfkg

[Serializable]
public class InventoryItem
{
    //relavent inventory references, including a network wrapper of the data in this item
    public InventoryItemDataWrapper data;
    public int stackSize;

    //be sure to add on to the stack on creation
    public InventoryItem(InventoryItemDataWrapper source){
        data = source;
        AddToStack();
    }

    //handles when a stack of items has an item added or removed
    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
