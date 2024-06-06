using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : MonoBehaviour
{
    [SerializeField]
    private InventorySystem myInventory;
    [SerializeField]
    private TextMeshProUGUI pickUpItemText;
    [SerializeField]
    public InputAction pickUp;

    private List<GameObject> lastItemTouched;

    public void OnEnable()
    {
        lastItemTouched = new List<GameObject>();
        pickUp.Enable();
    }

    public void OnDisable()
    {
        pickUp.Disable();
    }

    public void Update()
    {
        pickUpLastItem();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            if (!lastItemTouched.Contains(other.gameObject))
            {
                if (myInventory.canAddItem(other.gameObject.GetComponent<ItemObject>().referenceItem))
                {
                    pickUpItemText.text = "Press E to pickup";
                }
                else
                {
                    pickUpItemText.text = "Inventory full :(";
                }
                lastItemTouched.Add(other.gameObject);
            }
        }
    }

    public void pickUpLastItem()
    {
        if (lastItemTouched.Count == 0 || pickUp.ReadValue<float>() != 1f) return;

        if (lastItemTouched[0].GetComponent<ItemObject>().canPickUp)
        {
            myInventory.PickUpItem(lastItemTouched[0].GetComponent<ItemObject>().referenceItem, lastItemTouched[0]);
            lastItemTouched.Remove(lastItemTouched[0]);
            pickUpItemText.text = "";
        }
    }


    public void OnTriggerExit(Collider other)
    {
        pickUpItemText.GetComponent<TextMeshProUGUI>().text = "";
        if (other.gameObject.tag == "item")
        {
            lastItemTouched.Remove(other.gameObject);
        }
    }
}
