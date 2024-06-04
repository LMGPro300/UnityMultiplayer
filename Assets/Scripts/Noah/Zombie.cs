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
    CountdownTimer timer;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyAgent.destination = player.position;
        if (isAttackingPlayer)
        {
            timer.Tick(Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttackingPlayer && other.gameObject.tag == "player")
        {
            Debug.Log("started colliding with player");
            isAttackingPlayer = true;
            timer = new CountdownTimer(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
            timer.OnTimerStop += () => { DealPlayerDamage(enemyData.damagePerHit); };
            timer.Start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isAttackingPlayer && other.gameObject.tag == "player")
        {
            isAttackingPlayer = false;
            timer.Stop();
        }
    }

    private void DealPlayerDamage(float damage)
    {
        health.TakeDamage(damage);
    }
}
