using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: AmmoData.cs
 * Author: Noah Levy
 * What the program does: houses static properties of ammo, allows for scalability
 */

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/New Ammo Data")]
public class AmmoData : ScriptableObject
{
    //simple variables, can be modified in the editor
    public string type = "medium";
    public int packSize = 10;
}
