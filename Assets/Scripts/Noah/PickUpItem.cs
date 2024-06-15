using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Program name: PickUpItem.cs
 * Author: Noah Levy
 * What the program does: script is attached to a hitbox that handles picking up items
 */

public class PickUpItem : MonoBehaviour{

    //relavent inventory/ui references
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private TextMeshProUGUI pickUpText;
    private List<GameObject> lastItemTouched;

    //create a list of previously touched objects so they can all be picked up at once
    public void Awake(){
        lastItemTouched = new List<GameObject>();
    }

    //when entering an item hitbox, check if you can add it, and add it
    //otherwise display the appropriate error message
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
                //keep track of last item touched
                lastItemTouched.Add(realObject);
            }
        }
    }

    //when exiting an item hitbox, check if you can remove it, and remove it
    //update UI accordingly
    public void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "item"){
            ActualParent actualObject = other.gameObject.GetComponent<ActualParent>();
            GameObject realObject = other.gameObject;
            if (actualObject != null){
                realObject = actualObject.actualParent;
            }
            pickUpText.text = "";
            //remove the item
            lastItemTouched.Remove(realObject);
        }
    }

    //handles pickup logic when user gives an input
    public void RecievePickUpInput(float pickUpInput) {
        //makes sure we can actually pick up an item
        if (lastItemTouched.Count == 0 || pickUpInput != 1f) return;
        
        //picks up one item from our last touched items
        //checks if its null to handle weird cases with hitboxes
        GameObject lastItem = lastItemTouched[0];
        if (lastItem == null){
            lastItemTouched.Remove(lastItem);
            pickUpText.text = "";  
            return;
        }

        //checks if we can pick up our previous item object
        ItemObject lastItemObject = lastItem.GetComponent<ItemObject>();

        //remove the last item touched from our list of available items to be picked up
        if (!lastItemObject.canPickUp) return;
        lastItemTouched.Remove(lastItem);
        inventorySystem.PickUpItem(lastItemObject.referenceItem, lastItem);
        
        //update UI text accordingly
        pickUpText.text = "";
        lastItemObject.canPickUp = false;
        lastItemObject.pickupCooldown.Start();
    }


   
}
