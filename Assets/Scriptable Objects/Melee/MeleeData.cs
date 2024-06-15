using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee", menuName = "ScriptableObjects/New Melee")]
public class MeleeData : ScriptableObject
{
    public float damage;
    public float cooldown;
    public string meleeEquipAnimation;
    public string meleeSwingAnimation;
}
