using UnityEngine;
/*
 * Program name: IShootAble.cs
 * Author: ELvin Shen
 * What the program does: An interface that is added to entity components that make it register raycasts
 */

public interface IShootAble{
    void IsShot(float damage);
    
    string hitType();

    bool isAlive();

    void DoRagdoll(Vector3 force, Vector3 hitLocation);
}