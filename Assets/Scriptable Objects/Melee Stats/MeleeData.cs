using Eflatun.SceneReference.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "melee data")]
public class MeleeData : ScriptableObject
{
    public float damage;
    public float cooldown;
}
