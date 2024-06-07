using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    [SerializeField]
    public EnemyData enemyData;
    [SerializeField]
    public PlayerHealth playerHealth;
    CountdownTimer timer;

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
        if (other.gameObject.tag == "Player")
        {
            isAttackingPlayer = true;
            timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
            timer.Start();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isAttackingPlayer = false;
            timer.Stop();
        }
    }

    private void DealPlayerDamage(float damage)
    {
        playerHealth.TakeDamage(damage);
        timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
        timer.Start();
    }
}
