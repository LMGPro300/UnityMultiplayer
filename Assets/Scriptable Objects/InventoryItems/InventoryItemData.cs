using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item")]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
    public GameObject displayPrefab;
    public GameObject globalPrefab;
    public int maxStackSize = 1;
    public bool isGun;
    public bool isMelee;
    public GunScriptableObject weapon = null;
    public MeleeData melee = null;
    public string animation = "Armature|PickUp";
    public float sellPrice = 10f;
}
