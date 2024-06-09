using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum AmmoType{
    smallAmmo,
    mediumAmmo,
    largeAmmo
}


[CreateAssetMenu(fileName = "New Gun", menuName = "ScriptableObjects/New Gun")]
public class GunScriptableObject : ScriptableObject
{
    public int magSize;
    public int currentMagSize = 0;
    //public int ammoCapacity;
    public float damage;
    public float firerate;
    public float reloadTime;
    public float force;
    public DictionarySerializer EntityComponents;
    public string shootingAnimation;
    public string reloadAnimation;
    public string gunReloadAnimation;
    public string equipAnimation;
    public string gunEquipAnimation;
    public AmmoType ammoType;
    

}


