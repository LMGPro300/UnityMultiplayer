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

    GameObject pastObject = null;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy" && pastObject == null)
        {
            canAttackPlayer = true;
            pastObject = other.gameObject;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy" && other.gameObject == pastObject)
        {
            canAttackPlayer = false;
            pastObject = null;
        }
    }

    public void Awake()
    {
        playerClick.Enable();
        playerClick.performed += Attack;
        attackTimer = new CountdownTimer(1f);
        attackTimer.OnTimerStop += timerStuffs;
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
            Entity collidedEntity = pastObject.GetComponent<EntityComponent>().parentEntity;
            if (collidedEntity.isAlive)
            {
                pastObject.GetComponent<EntityComponent>().IsShot(meleeData.damage);
                if (!collidedEntity.isAlive)
                {
                    collidedEntity.DoRagdoll(playerTransform.forward * 2f, playerTransform.position);
                    pastObject = null;
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
