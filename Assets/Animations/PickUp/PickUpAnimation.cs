using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform itemLocation;
    //[SerializeField] private GameObject noItem;

    public GameObject previousDisplayItem;

    public void changeSlot(GameObject prefab){
        //if switching to empty hotbar slot
        if (prefab == null)
        {
            //if already rendering no item, return
            if (previousDisplayItem == null) { return; }
            //otherwise destroy the previous item rendered
            Destroy(previousDisplayItem);
            previousDisplayItem = null;
            return;
        }
        //if switching from a previously rendered item
        if (previousDisplayItem != null)
        {
            Destroy(previousDisplayItem);
        }
        //create and render new item, remembering to store it as a previous display item
        GameObject droppedItem = Instantiate(prefab, itemLocation);
        animator.Play("Armature|PickUp");
        previousDisplayItem = droppedItem;
    }

}
