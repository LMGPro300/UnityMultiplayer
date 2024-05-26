using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public InventoryItemData referenceItem;
    [SerializeField]
    public GameObject myInventory;
    InventorySystem system;

    public void Start()
    {
        system = myInventory.GetComponent<InventorySystem>();
    }

    public void pickUpItem()
    {
        system.Add(referenceItem);
        Destroy(gameObject);
    }
}
