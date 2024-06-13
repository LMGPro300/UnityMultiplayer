using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Entity: MonoBehaviour{

    [SerializeField] private EnemyScriptableObject enemyScriptableObject;
    [SerializeField] private RagdollController ragdollController;
    [SerializeField] private List<GameObject> disabledObjectsWhenRagdolling;
    [SerializeField] private GameObject parent;
    [SerializeField] private int despawnTimerSeconds = 5;
    //[SerializeField] private AudioSource audioSource;
    //[SerializeField] private Animator shootingAnimation;
    //[SerializeField] private NormalEnemy enemyBehavior;
    public bool isAlive;
    private float health;
    public Action OnDeath = delegate { };

    public void Awake(){
        health = enemyScriptableObject.health;
        isAlive = true;
    }

    public void Start(){
    }

    public void takeDamage(float damage){
        this.health -= damage;
        //StartCoroutine(PlaySounds());
        Debug.Log(damage + " taken damage");
        if (health <= 0){
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
        ragdollController.TriggerRagdoll(force, hitLocation);
        gameObject.transform.parent = null;
        foreach (GameObject go in disabledObjectsWhenRagdolling)
        {
            Destroy(go);
        }
    }
}