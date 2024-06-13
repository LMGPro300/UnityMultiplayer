using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MeleeManager : MonoBehaviour
{
    [SerializeField]
    private BoxCollider meleeHitbox;
    [SerializeField]
    private InputAction playerClick;
    [SerializeField]
    private MeleeData meleeData;
    [SerializeField]
    private Transform playerTransform;

    CountdownTimer attackTimer;

    bool canAttackPlayer = false;
    bool coolDownDone = true;

    List<Entity> availableEnemies;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            Entity myEntity = other.gameObject.GetComponent<EntityComponent>().parentEntity;
            Debug.Log(myEntity + " NOAH LEVY");
            if (myEntity != null && !availableEnemies.Contains(myEntity))
            {
                canAttackPlayer = true;
                availableEnemies.Add(myEntity);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy" && availableEnemies.Contains(other.gameObject.GetComponent<EntityComponent>().parentEntity))
        {
            availableEnemies.Remove(other.gameObject.GetComponent<EntityComponent>().parentEntity);
            if (availableEnemies.Count == 0)
            {
                canAttackPlayer = false;
            }
        }
    }

    public void Awake()
    {
        playerClick.Enable();
        playerClick.performed += Attack;
        attackTimer = new CountdownTimer(1f);
        attackTimer.OnTimerStop += timerStuffs;
        availableEnemies = new List<Entity>();
    }

    public void Update()
    {
        if (canAttackPlayer && !coolDownDone)
        {
            attackTimer.Tick(Time.deltaTime);
        }
    }

    public void OnDisable()
    {
        playerClick.Disable();
        attackTimer.Stop();
        playerClick.performed -= Attack;
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (canAttackPlayer && coolDownDone && meleeData != null)
        {
            for (int i = availableEnemies.Count-1; i >= 0; i--)
            {
                Entity obj = availableEnemies[i];
                if (obj.isAlive)
                {
                    obj.takeDamage(meleeData.damage);
                    if (!obj.isAlive)
                    {
                        obj.DoRagdoll(playerTransform.forward * 2f, playerTransform.position);
                        availableEnemies.Remove(obj);
                    }
                }
            }
            attackTimer.SetNewTime(meleeData.cooldown);
            attackTimer.Start();
            coolDownDone = false;
        }
    }
    
    public void timerStuffs()
    {
        coolDownDone = true;
    }

    public void ChangeSlot(MeleeData newData)
    {
        meleeData = newData;
    }
}
