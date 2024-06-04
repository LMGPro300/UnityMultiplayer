using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relocator : MonoBehaviour
{
    [SerializeField] public GameObject otherLocation;

    public void SetOtherLocation(GameObject otherLocation){
        this.otherLocation = otherLocation;
    }

    public GameObject GetOtherLocation(){
        return this.otherLocation;
    }
}
