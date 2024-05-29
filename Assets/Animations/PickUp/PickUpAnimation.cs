using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform itemLocation;


    public void changeSlot(GameObject prefab){
        GameObject droppedItem = Instantiate(prefab, itemLocation);
        animator.Play("Armature|PickUp");
    }

}
