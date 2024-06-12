using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/New Ammo Data")]
public class AmmoData : ScriptableObject
{
    public string type = "medium";
    public int packSize = 10;
}
