using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Program name: EnemyAttack.cs
 * Author: Noah Levy
 * What the program does: houses static properties of ammo, allows for scalability
 */

public class EnemyAttack : MonoBehaviour
{
    //create necessary references
    [SerializeField]
    public EnemyScriptableObject enemyData;
    [SerializeField] private Animator zombieAnimatior;
    CountdownTimer timer;

    private PlayerHealth healthToDamage;


    bool isAttackingPlayer = false;

    public void Update()
    {
        if (isAttackingPlayer)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    //deal damage when timer finishes cooldown
    public void Awake()
    {
        timer = new CountdownTimer(1f);
        timer.OnTimerStop += () => { DealPlayerDamage(enemyData.damagePerHit); };
    }

    public void OnTriggerEnter(Collider other)
    {
        //handles attacking player if in the trigger zone
        if (other.gameObject.tag == "enemy stop zone")
        {
            healthToDamage = other.transform.parent.gameObject.GetComponent<PlayerHealth>();
            isAttackingPlayer = true;
            zombieAnimatior.Play("Default|ZombieAttack");
            timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
            timer.Start();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //handles attacking player if exits the trigger zone
        if (other.gameObject.tag == "enemy stop zone")
        {
            isAttackingPlayer = false;
            timer.Stop();
        }
    }

    private void DealPlayerDamage(float damage)
    {
        //deal player damage based on scriptable object of this enemy and take a random amount of damage
        healthToDamage.TakeDamage(damage);
        timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
        timer.Start();
    }
}
