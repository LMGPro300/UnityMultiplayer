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
    private MeleeData meleeData;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField] private Animator armAnimator;

    private Animator meleeAnimator;
    

    CountdownTimer attackTimer;

    bool canAttackPlayer = false;
    bool coolDownDone = true;

    List<Entity> availableEnemies;

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

    public void Awake()
    {
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


    public void Attack(float shootInput)
    {
        if (shootInput == 0f) return;
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

    public void GetItemAnimator(Animator itemAnimator){
        meleeAnimator = itemAnimator;
    }
}
