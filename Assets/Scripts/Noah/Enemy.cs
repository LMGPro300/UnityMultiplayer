using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public EnemyData enemyData;
    public PlayerHealth health;
    public float currentHealth;

    NavMeshAgent enemyAgent;
    bool isStopped = false;
    bool isInRange = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        currentHealth = enemyData.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && !isStopped) {
            enemyAgent.destination = player.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy agro zone")
        {
            isInRange = true;
            enemyAgent.SetDestination(player.position);
        }
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = true;
            enemyAgent.SetDestination(enemyAgent.gameObject.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy agro zone" && isInRange)
        {
            isInRange = false;
            enemyAgent.SetDestination(enemyAgent.gameObject.transform.position);
            return;
        }
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = false;
            enemyAgent.SetDestination(player.position);
        }
    }

    public void DealZombieDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("ouch");
    }
}
