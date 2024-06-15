using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
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

    public void Awake()
    {
        timer = new CountdownTimer(1f);
        timer.OnTimerStop += () => { DealPlayerDamage(enemyData.damagePerHit); };
    }

    public void OnTriggerEnter(Collider other)
    {
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
        if (other.gameObject.tag == "enemy stop zone")
        {
            isAttackingPlayer = false;
            timer.Stop();
        }
    }

    private void DealPlayerDamage(float damage)
    {
        healthToDamage.TakeDamage(damage);
        timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
        timer.Start();
    }
}
