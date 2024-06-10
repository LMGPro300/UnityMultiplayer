using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

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

    GameObject lastCollidedEnemy = null;
    List<GameObject> pastObjects = new List<GameObject>();

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            canAttackPlayer = true;
            lastCollidedEnemy = other.gameObject;
            pastObjects.Add(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            canAttackPlayer = false;
            lastCollidedEnemy = null;
            if (pastObjects.Contains(other.gameObject))
            {
                pastObjects.Remove(other.gameObject);
            }
        }
    }

    public void Awake()
    {
        playerClick.Enable();
        playerClick.performed += Attack;
        attackTimer = new CountdownTimer(1f);
    }

    public void Update()
    {
        if (canAttackPlayer)
        {
            attackTimer.Tick(Time.deltaTime);
        }
    }

    public void OnDisable()
    {
        playerClick.Disable();
    }

    public void Attack(InputAction.CallbackContext ctx)
    {
        if (canAttackPlayer && attackTimer.IsFinished() && meleeData != null)
        {
            foreach (GameObject go in pastObjects) {
                Entity collidedEntity = go.GetComponent<Entity>();
                if (collidedEntity.isAlive)
                {
                    collidedEntity.takeDamage(meleeData.damage);
                    if (!collidedEntity.isAlive)
                    {
                        collidedEntity.DoRagdoll(playerTransform.forward * 2f, playerTransform.position);
                    }
                }
            }
            attackTimer.SetNewTime(meleeData.cooldown);
            attackTimer.Start();
        }
    }

    public void ChangeSlot(MeleeData newData)
    {
        meleeData = newData;
    }
}
