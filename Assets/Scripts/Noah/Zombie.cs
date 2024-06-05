using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public Transform player;
    public EnemyData enemyData;
    public PlayerHealth health;

    NavMeshAgent enemyAgent;
    bool isAttackingPlayer = false;
    bool isInRange = false;
    CountdownTimer timer;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        timer = new CountdownTimer(1f);
        timer.OnTimerStop += () => { DealPlayerDamage(enemyData.damagePerHit); };
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && !isAttackingPlayer) {
            enemyAgent.destination = player.position;
        }
        else if (isAttackingPlayer)
        {
            timer.Tick(Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "tag")
        {
            isInRange = true;
            enemyAgent.SetDestination(player.position);
        }
        if (!isAttackingPlayer && other.gameObject.tag == "Player")
        {
            DealPlayerDamage(10f);
            isAttackingPlayer=true;
            timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
            timer.Start();
            enemyAgent.SetDestination(enemyAgent.gameObject.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "tag" && isInRange)
        {
            isInRange = false;
            enemyAgent.SetDestination(enemyAgent.gameObject.transform.position);
            return;
        }
        if (isAttackingPlayer && other.gameObject.tag == "Player")
        {
            isAttackingPlayer = false;
            enemyAgent.SetDestination(enemyAgent.gameObject.transform.position);
        }
    }

    private void DealPlayerDamage(float damage)
    {
        health.TakeDamage(damage);
        timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
        timer.Start();
    }
}
