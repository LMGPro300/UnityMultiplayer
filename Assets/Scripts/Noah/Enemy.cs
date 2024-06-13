using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNav : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    private PlayerHealth playerHealth;
    private NavMeshAgent enemyAgent;
    private bool isStopped = false;
    private bool isInRange = false;
    private Transform myPosition = null;

    // Start is called before the first frame update
    void Start()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && !isStopped)
        {
            enemyAgent.destination = PlayerManager.Instance.playersTransform[0].position;
            Debug.Log("trying to move towards player");
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
}
