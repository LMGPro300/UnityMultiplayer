using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
        //Debug.Log("<color=green>landed ground</color>");
public class PlayerCollision : NetworkBehaviour//MonoBehaviour//
{
    [SerializeField] private int jumpFrameBuffer = 100;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private TextMeshProUGUI pickUpItemText;

    private bool isGrounded = false;
    private int currentJumpFrame;
    List<GameObject> lastItemTouched;

    private void Awake(){
        lastItemTouched = new List<GameObject>();
    }
    
    private void OnTriggerEnter(Collider other){
        string gotTag = other.gameObject.tag;
        if (gotTag == "ground"){
            currentJumpFrame = jumpFrameBuffer;
            isGrounded = true;
        } else if (gotTag == "item"){
            if (!lastItemTouched.Contains(other.gameObject)){
                if (inventorySystem.canAddItem(other.gameObject.GetComponent<ItemObject>().referenceItem)){
                    pickUpItemText.text = "Press E to pickup";
                }
                else{
                    pickUpItemText.text = "Inventory full :(";
                }
                lastItemTouched.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other){
        string gotTag = other.gameObject.tag;
        if (gotTag == "ground"){
            if (currentJumpFrame != 0){
                currentJumpFrame -= 1;
            }
            isGrounded = true;
        } else if (gotTag == "Funny Slope"){
            isGrounded = false;
        }
    }

    private void OnTriggerExit(Collider other){
        string gotTag = other.gameObject.tag;
        if (gotTag == "ground"){
            isGrounded = false;
        } else if (gotTag == "item"){
            pickUpItemText.text = "";
            lastItemTouched.Remove(other.gameObject);
        }
    }

    public void DidJump(){
        isGrounded = false;
    }

    public bool LastItemTouchedIsEmpty(){
        return lastItemTouched.Count == 0;
    }

    public void PickUpLastItemTouched(){
        GameObject lastItem = lastItemTouched[0];
        if (lastItem == null){
            lastItemTouched.Remove(lastItem);
            pickUpItemText.text = "";  
            return;
        }
        ItemObject lastItemObject = lastItem.GetComponent<ItemObject>();

        if (!lastItemObject.canPickUp) return;
        lastItemTouched.Remove(lastItem);
        inventorySystem.PickUpItem(lastItemObject.referenceItem, lastItem);
        
        pickUpItemText.text = "";
        lastItemObject.canPickUp = false;
        lastItemObject.pickupCooldown.Start();
    }

    public bool GetIsGrounded(){
        return isGrounded;
    }

    public bool GetIsAir(){
        return isGrounded && currentJumpFrame == 0;
    }
}
