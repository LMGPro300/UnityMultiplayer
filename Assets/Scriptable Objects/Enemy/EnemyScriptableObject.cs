using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: EnemyScriptableObject.cs
 * Author: Noah Levy
 * What the program does: houses static properties of enemies, allows for scalability
 */

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/New Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    //simple variables that can be changed in the inspector
    //public Animator zombieAnimationLogic;
    public float damagePerHit;
    public float minDamageWaitCooldown = 0.1f;
    public float maxDamageWaitCooldown = 0.9f;
    public float minSpeed = 3.5f;
    public float maxSpeed = 7f;
    public float health = 100f;
}
