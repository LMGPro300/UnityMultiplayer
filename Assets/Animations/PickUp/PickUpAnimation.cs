using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickUpAnimation : NetworkBehaviour
{
    [SerializeField] private Animator armAnimator;
    [SerializeField] private Animator charAnimator;
    [SerializeField] private Transform itemLocation;
    [SerializeField] private PlayerShoot playerShoot;
    //[SerializeField] private GameObject noItem;

    public GameObject previousDisplayItem;

    public void Awake()
    {
        previousDisplayItem = null;
    }

    public void changeSlot(GameObject prefab, string _animation){
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
        Animator itemAnimator =  droppedItem.GetComponent<Animator>();
        if (itemAnimator != null){
            playerShoot.GetItemAnimator(itemAnimator);
        }
        //armAnimator.Play(null);
        //armAnimator.Play(animation);
        //charAnimator.Play(null);
        //charAnimator.Play(null);
        
        charAnimator.Play(_animation);
        //charAnimator.Play(null);
        
        //.transform.position = itemLocation.position;
        //droppedItem.transform.rotation = itemLocation.rotation;
        previousDisplayItem = droppedItem;
    }

}
