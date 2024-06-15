using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*
 * Program name: Entity.cs
 * Author: Elvin Shen
 * What the program does: Allows to make anything into an "Entity" (capable of taking damage and death) when this component it added to it
 */

public class Entity: MonoBehaviour{

    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    [SerializeField] private RagdollController ragdollController;
    [SerializeField] private GameObject ragDollToKeep;
    [SerializeField] private List<GameObject> disabledObjectsWhenRagdolling;
    [SerializeField] private List<Behaviour> disabledComponentsWhenRagdolling;
    [SerializeField] private int despawnTimerSeconds = 5;
    //[SerializeField] private AudioSource audioSource;
    //[SerializeField] private Animator shootingAnimation;
    //[SerializeField] private NormalEnemy enemyBehavior;
    public bool isAlive;
    private float health;
    public Action OnDeath = delegate { };

    public void Awake(){
        //Define the stats of the enemy
        health = enemyScriptableObject.health;
        isAlive = true;
    }

    public void Start(){
    }

    public void takeDamage(float damage){
        //This is called when damage is taken, an OnDeath delegate is invoked and is no longer alive
        this.health -= damage;
        //StartCoroutine(PlaySounds());
        Debug.Log(damage + " taken damage");
        if (health <= 0 && isAlive == true){
            isAlive = false; 
            OnDeath.Invoke();   
            //shootingAnimation.enabled = false;
            //enemyBehavior.enabled = false;
        }
    }

    /*

    private IEnumerator PlaySounds() {
        yield return new WaitForSeconds(0.15f);
        audioSource.Play();
    }

    */

    public void DoRagdoll(Vector3 force, Vector3 hitLocation){
        //Applies a force to a kinematic body that gives it the "ragdoll" effect certain gameobjects are disabled
        // such as navmesh (avoid moving during ragdoll) and the hurtbox
        ragdollController.TriggerRagdoll(force, hitLocation);
        ragdollController.transform.SetParent(gameObject.transform);
        foreach (GameObject go in disabledObjectsWhenRagdolling)
        {
            go.SetActive(false);
        }
        foreach(Behaviour be in disabledComponentsWhenRagdolling)
        {
            be.enabled = false;
        }
    }
}