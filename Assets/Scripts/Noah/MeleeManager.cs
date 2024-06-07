using System.Collections;
using System.Collections.Generic;
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

    CountdownTimer attackTimer;

    bool canAttackPlayer = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            canAttackPlayer = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            canAttackPlayer = false;
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
        if (canAttackPlayer)
        {
            if (attackTimer.IsFinished())
            {
                //apply damage
                Debug.Log(meleeData.damage + " damage was done to the zombie");

                attackTimer.SetNewTime(meleeData.cooldown);
                attackTimer.Start();
            }
        }
    }
}
