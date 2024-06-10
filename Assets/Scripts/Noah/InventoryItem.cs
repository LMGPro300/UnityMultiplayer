using System;

[Serializable]
public class InventoryItem
{
    public InventoryItemData data;
    public int stackSize;
    public DynamicItemData changingData = null;

    public InventoryItem(InventoryItemData source, DynamicItemData myDynamicData)
    {
        data = source;
        changingData = myDynamicData;
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
