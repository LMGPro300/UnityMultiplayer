using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "weapon")]
public class GunData : ScriptableObject
{
    public int AmmoCapacity = 10;
    public int WeaponCapacity = 10;
    public int bulletRange = 1000;
    public int damage = 10;
}
