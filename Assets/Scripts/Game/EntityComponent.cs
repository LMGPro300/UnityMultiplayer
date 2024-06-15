using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: EntityComponent.cs
 * Author: Elvin Shen 
 * What the program does: A "body part" of an Entity that allows the overall entity to take damage
 */

public class EntityComponent : MonoBehaviour, IShootAble
{
    // Start is called before the first frame update
    [SerializeField] public Entity parentEntity;
    [SerializeField] private EntityComponentScriptableObject entityComponentScriptableObject;

    //When it is shot
    public void IsShot(float damage){
        parentEntity.takeDamage(damage);
    }

    //The hit type, referening to body part or armour hit
    public string hitType(){
        return entityComponentScriptableObject.hitType;
    }

    public bool isAlive(){
        return parentEntity.isAlive;
    }

    //trigger the ragdoll on the whole entity
    public void DoRagdoll(Vector3 force, Vector3 hitLocation){
        parentEntity.DoRagdoll(force, hitLocation);
    }
}
