using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(menuName = "item data")]
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
    public GunData weapon = null;
    public MeleeData melee = null;
}
