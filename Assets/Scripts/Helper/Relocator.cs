using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: Relocator.cs
 * Author: Elvin Shen
 * What the program does: Relocates to another gameobject (probably far away)
 */

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
