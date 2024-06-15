using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/*
 * Program name: MeleeManager.cs
 * Author: Noah Levy, slight help from tutorial
 * Handles attacking of enemies
 */

public class MeleeManager : MonoBehaviour
{
    //relavent references
    [SerializeField]
    private BoxCollider meleeHitbox;
    [SerializeField]
    private MeleeData meleeData;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField] private Animator armAnimator;

    private Animator meleeAnimator;
    

    CountdownTimer attackTimer;

    bool canAttackPlayer = true;
    bool coolDownDone = true;

    List<Entity> availableEnemies;

    //if can attacking enemy, update respective flags
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            EntityComponent myEntityComp = other.gameObject.GetComponent<EntityComponent>();
            if (myEntityComp == null) return;
            Entity myEntity = myEntityComp.parentEntity;
            Debug.Log(myEntity + " NOAH LEVY");
            if (myEntity != null && !availableEnemies.Contains(myEntity))
            {
                canAttackPlayer = true;
                availableEnemies.Add(myEntity);
            }
        }
    }

    //if leaving enemy, update respective flags
    public void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "enemy"){
            EntityComponent ECom = other.gameObject.GetComponent<EntityComponent>();
            if (ECom != null && !availableEnemies.Contains(ECom.parentEntity)){
                availableEnemies.Remove(ECom.parentEntity);
                if (availableEnemies.Count == 0)
                {
                    canAttackPlayer = false;
                }
            }
        }
    }

    //create attack timer
    public void Awake()
    {
        attackTimer = new CountdownTimer(1f);
        attackTimer.OnTimerStop += timerStuffs;
        availableEnemies = new List<Entity>();
    }

    //tick timer
    public void Update()
    {
        if (canAttackPlayer && !coolDownDone)
        {
            attackTimer.Tick(Time.deltaTime);
        }
    }
    
    //handle attack input
    public void Attack(float shootInput)
    {
        if (shootInput == 0f) return;
        //play attack animation and deal damage
        if (meleeData != null && coolDownDone && canAttackPlayer){
            armAnimator.Play(null);
            armAnimator.Play(meleeData.meleeSwingAnimation);
            if (meleeAnimator != null){
                meleeAnimator.Play(null);
                meleeAnimator.Play(meleeData.meleeSwingAnimation);
            }
        }
        
        if (canAttackPlayer && coolDownDone && meleeData != null)
        {
            //loop through range of enemies player can attack, killing off enemies if they run out of health
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
    
    //allow attacking
    public void timerStuffs()
    {
        coolDownDone = true;
    }

    //handle if inventory changes to a melee weapon
    public void ChangeSlot(MeleeData newData)
    {
        if (newData == null){
            meleeData = null;
            return;
        }
        meleeData = newData;
        if (meleeAnimator != null){
            meleeAnimator.Play(null);
            meleeAnimator.Play(meleeData.meleeEquipAnimation);
        }
    }

    //gets needed animator
    public void GetItemAnimator(Animator itemAnimator){
        meleeAnimator = itemAnimator;
    }
}
