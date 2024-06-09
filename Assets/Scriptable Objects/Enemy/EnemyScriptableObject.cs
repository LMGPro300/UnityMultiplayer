using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/New Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    //public Animator zombieAnimationLogic;
    public float damagePerHit;
    public float minDamageWaitCooldown = 0.1f;
    public float maxDamageWaitCooldown = 0.9f;
    public float health = 100f;
}
