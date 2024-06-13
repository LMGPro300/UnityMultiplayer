using System;

[Serializable]
public class InventoryItem
{
    public InventoryItemDataWrapper data;
    public int stackSize;

    public InventoryItem(InventoryItemDataWrapper source){
        data = source;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
