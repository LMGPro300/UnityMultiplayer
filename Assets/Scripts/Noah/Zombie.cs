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
        timer = new CountdownTimer(1f);
        timer.OnTimerStop += () => { DealPlayerDamage(enemyData.damagePerHit); };
    }

    // Update is called once per frame
    void Update()
    {
        enemyAgent.destination = new Vector3(player.position.x, transform.position.y, player.position.z);
        if (isAttackingPlayer)
        {
            timer.Tick(Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isAttackingPlayer && other.gameObject.tag == "Player")
        {
            DealPlayerDamage(10);
            isAttackingPlayer = true;
            timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
            timer.Start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("the player is no longer being attacked");
        if (isAttackingPlayer && other.gameObject.tag == "Player")
        {
            isAttackingPlayer = false;
            timer.Stop();
        }
    }

    private void DealPlayerDamage(float damage)
    {
        health.TakeDamage(damage);
        timer.SetNewTime(Random.Range(enemyData.minDamageWaitCooldown, enemyData.maxDamageWaitCooldown));
        timer.Start();
    }
}
