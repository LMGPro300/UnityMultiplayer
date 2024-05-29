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
        if (previousDisplayItem != null)
        {
            Destroy(previousDisplayItem);
        }
        GameObject droppedItem = Instantiate(prefab, itemLocation);
        animator.Play("Armature|PickUp");
        previousDisplayItem = droppedItem;
    }

}
