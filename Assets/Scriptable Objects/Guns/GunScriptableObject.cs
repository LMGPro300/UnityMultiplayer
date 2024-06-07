using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "ScriptableObjects/New Gun")]
public class GunScriptableObject : ScriptableObject
{
    public int magSize;
    //public int ammoCapacity;
    public float damage;
    public float firerate;

    public float force;
    public DictionarySerializer EntityComponents;

}


