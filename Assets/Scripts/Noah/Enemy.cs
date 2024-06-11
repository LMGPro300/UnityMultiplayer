using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform player;
    public EnemyScriptableObject enemyData;
    public PlayerHealth health;
    public float currentHealth;

    NavMeshAgent enemyAgent;
    bool isStopped = false;
    bool isInRange = false;

    Transform myPosition = null;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        currentHealth = enemyData.health;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && !isStopped)
        {
            enemyAgent.destination = PlayerManager.Instance.playersTransform[0].position;
        }
        else
        {
            enemyAgent.destination = enemyAgent.gameObject.transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Transform temp = PlayerManager.Instance.playersTransform[0];
        if (other.gameObject.tag == "enemy agro zone")
        {
            isInRange = true;
            myPosition = PlayerManager.Instance.playersTransform[0];
        }
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = true;
            myPosition = enemyAgent.gameObject.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Transform temp = PlayerManager.Instance.playersTransform[0];
        if (other.gameObject.tag == "enemy agro zone" && isInRange)
        {
            isInRange = false;
            myPosition = enemyAgent.gameObject.transform;
            return;
        }
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = false;
            myPosition = PlayerManager.Instance.playersTransform[0];
        }
    }

    public void DealZombieDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("ouch");
    }
}
