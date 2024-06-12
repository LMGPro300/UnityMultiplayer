using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    private NavMeshAgent enemyAgent;
    private bool isStopped = false;
    private bool isInRange = false;
    private Transform targetedPlayer = null;

    List<Transform> reachablePlayers;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        transform.localEulerAngles = new Vector3(0, 180, 0);
        reachablePlayers = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && !isStopped && PlayerManager.Instance.playersTransform.Count > 0)
        {
            enemyAgent.destination = targetedPlayer.position;
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
        //Transform temp = PlayerManager.Instance.playersTransform[0];
        if (other.gameObject.tag == "enemy agro zone" && isInRange)
        {
            isInRange = false;
            reachablePlayers.Remove(other.transform.parent);
            if (reachablePlayers.Count > 0 )
            {
                targetedPlayer = targetNearestPlayer(gameObject.transform.parent);
            }
            return;
        }
        if (other.gameObject.tag == "enemy stop zone")
        {
            isStopped = false;
        }
    }

    public Transform targetNearestPlayer(Transform curPos)
    {
        float nearestDist = float.MaxValue;
        Transform bestPlayer = null;
        foreach (Transform playerPos in reachablePlayers)
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
