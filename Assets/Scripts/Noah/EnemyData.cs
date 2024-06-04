using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public Animator zombieAnimationLogic;
    public float damagePerHit;
    public float minDamageWaitCooldown = 0.1f;
    public float maxDamageWaitCooldown = 0.9f;
}
