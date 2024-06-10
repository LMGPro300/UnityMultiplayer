using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpItem : MonoBehaviour{
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private TextMeshProUGUI pickUpText;
    private List<GameObject> lastItemTouched;

    public void Awake(){
        lastItemTouched = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == "item"){
            ActualParent actualObject = other.gameObject.GetComponent<ActualParent>();
            GameObject realObject = other.gameObject;
            if (actualObject != null){
                realObject = actualObject.actualParent;
            }
            if (!lastItemTouched.Contains(realObject)){
                if (inventorySystem.canAddItem(realObject.GetComponent<ItemObject>().referenceItem)){
                    pickUpText.text = "Press E to pickup";
                }
                else{
                    pickUpText.text = "Inventory full :(";
                }
                lastItemTouched.Add(realObject);
            }
        }
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "item"){
            ActualParent actualObject = other.gameObject.GetComponent<ActualParent>();
            GameObject realObject = other.gameObject;
            if (actualObject != null){
                realObject = actualObject.actualParent;
            }
            pickUpText.text = "";
            lastItemTouched.Remove(realObject);
        }
    }

    public void RecievePickUpInput(float pickUpInput) {
        if (lastItemTouched.Count == 0 || pickUpInput != 1f) return;
        

        GameObject lastItem = lastItemTouched[0];
        if (lastItem == null){
            lastItemTouched.Remove(lastItem);
            pickUpText.text = "";  
            return;
        }

        ItemObject lastItemObject = lastItem.GetComponent<ItemObject>();

        if (!lastItemObject.canPickUp) return;
        lastItemTouched.Remove(lastItem);
        inventorySystem.PickUpItem(lastItemObject.referenceItem, lastItem, lastItemObject.changingData);
        
        pickUpText.text = "";
        lastItemObject.canPickUp = false;
        lastItemObject.pickupCooldown.Start();
    }
}
