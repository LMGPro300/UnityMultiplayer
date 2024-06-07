using UnityEngine;

public interface IShootAble{
    void IsShot(float damage);
    
    string hitType();

    bool isAlive();

    void DoRagdoll(Vector3 force, Vector3 hitLocation);
}