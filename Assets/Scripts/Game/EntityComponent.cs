using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityComponent : MonoBehaviour, IShootAble
{
    // Start is called before the first frame update
    [SerializeField] public Entity parentEntity;
    [SerializeField] private EntityComponentScriptableObject entityComponentScriptableObject;


    public void IsShot(float damage){
        parentEntity.takeDamage(damage);
    }

    public string hitType(){
        return entityComponentScriptableObject.hitType;
    }

    public bool isAlive(){
        return parentEntity.isAlive;
    }

    public void DoRagdoll(Vector3 force, Vector3 hitLocation){
        parentEntity.DoRagdoll(force, hitLocation);
    }
}
