using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

/*
 * Program name: TabGroup.cs
 * Author: Noah Levy, some code taken from tutorial
 * What the program does: controls where this enemy navagates too
 */

//RELAVENT TUTORIAL: https://www.youtube.com/watch?v=u2EQtrdgfNs

public class Enemy : NetworkBehaviour
{
    //Define references to this enemies stats and how it should behave
    [SerializeField]
    public EnemyScriptableObject enemyData;
    [SerializeField]
    public bool caresAboutAgroZone = true;

    //Simple flags and navmesh objects for tracking reachable players and if enemy should move
    private NavMeshAgent enemyAgent;
    private bool isStopped = false;
    private bool isInRange = false;
    private Transform targetedPlayer = null;
    private List<Transform> reachablePlayers;

    // Start is called before the first frame update
    void Start()
    {
        //sets target position to player on startup
        if (!IsServer) return;
        enemyAgent = GetComponent<NavMeshAgent>();
        enemyAgent.speed = Random.Range(enemyData.minSpeed, enemyData.maxSpeed);
        reachablePlayers = new List<Transform>();
        targetedPlayer = targetNearestPlayer(gameObject.transform.parent);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        
        //check if we're already targeting a player and can still target them
        if (isInRange && !isStopped && PlayerManager.Instance.playersTransform.Count > 0)
        {
            enemyAgent.destination = targetedPlayer.position;
        }
        //find a new enemy to target
        else if (!isStopped && !caresAboutAgroZone)
        {
            enemyAgent.destination = targetNearestPlayer(gameObject.transform.parent).position;
        }
        //set wanted position to yourself, thus stopping
        else
        {
            enemyAgent.destination = enemyAgent.gameObject.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        //set target based on if enemy entered agro zone or stop zone
        if (other.gameObject.tag == "enemy agro zone")
        {
            isInRange = true;
            reachablePlayers.Add(other.transform.parent);
            targetedPlayer = targetNearestPlayer(gameObject.transform.parent);
        }
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        //set target based on if enemy exited agro zone or stop zone
        if (other.gameObject.tag == "enemy agro zone" && isInRange)
        {
            isInRange = false;
            reachablePlayers.Remove(other.transform.parent);
            //if there's still players to target, set your target position to them
            if (reachablePlayers.Count > 0 )
            {
                targetedPlayer = targetNearestPlayer(gameObject.transform.parent);
            }
            return;
        }
        //don't stop if leaving enemy stop zone
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = false;
        }
    }

    public Transform targetNearestPlayer(Transform curPos)
    {
        //used for netcode reasons
        //targets the nearest player that is within this enemy agro zone
        float nearestDist = float.MaxValue;
        Transform bestPlayer = curPos;
        foreach (Transform playerPos in PlayerManager.Instance.playersTransform)
        {
            if (Vector3.Distance(curPos.position, playerPos.position) < nearestDist)
            {
                nearestDist = Vector3.Distance(playerPos.position, curPos.position);
                bestPlayer = playerPos;
            }
        }
        return bestPlayer;
    }
}
