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
public class GunScriptableObject : ScriptableObject{
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

/*
[System.Serializable]
public class GunScriptableObjectWrapper{
    [SerializeField] public GunScriptableObject original;
    [SerializeField] public int magSize;
    [SerializeField] public int currentMagSize;
    [SerializeField] public float damage;
    [SerializeField] public float firerate;
    [SerializeField] public float reloadTime;
    [SerializeField] public float force;
    [SerializeField] public DictionarySerializer EntityComponents;
    [SerializeField] public string shootingAnimation;
    [SerializeField] public string reloadAnimation;
    [SerializeField] public string gunReloadAnimation;
    [SerializeField] public string equipAnimation;
    [SerializeField] public string gunEquipAnimation;
    [SerializeField] public AmmoType ammoType;

    public GunScriptableObjectWrapper(GunScriptableObject gunScriptableObject){
        original = gunScriptableObject;
        magSize = gunScriptableObject.magSize;
        currentMagSize = gunScriptableObject.currentMagSize;
        damage = gunScriptableObject.damage;
        firerate = gunScriptableObject.firerate;
        reloadTime = gunScriptableObject.reloadTime;
        force = gunScriptableObject.force;
        EntityComponents = gunScriptableObject.EntityComponents;
        shootingAnimation = gunScriptableObject.shootingAnimation;
        reloadAnimation = gunScriptableObject.reloadAnimation;
        gunReloadAnimation = gunScriptableObject.gunReloadAnimation;
        equipAnimation = gunScriptableObject.equipAnimation;
        gunEquipAnimation = gunScriptableObject.gunEquipAnimation;
        ammoType = gunScriptableObject.ammoType;
    }
}
*/

