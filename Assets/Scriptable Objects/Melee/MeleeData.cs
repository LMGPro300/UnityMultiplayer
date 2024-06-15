using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: MeleeData.cs
 * Author: Noah Levy
 * What the program does: houses static properties of melee weapons, allows for scalability
 */

[CreateAssetMenu(fileName = "New Melee", menuName = "ScriptableObjects/New Melee")]
public class MeleeData : ScriptableObject
{
    //simple variables that can be changed in the editor
    public float damage;
    public float cooldown;
    public string meleeEquipAnimation;
    public string meleeSwingAnimation;
}
