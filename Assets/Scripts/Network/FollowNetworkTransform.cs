using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: FollowNetworkTranform.cs
 * Author: Elvin Shen 
 * What the program does: Follow the tranform or another object, used since network object spawned
 *                        dynamically cannot be parented
 */

public class FollowNetworkTransform : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    public void SetTargetTransform(Transform targetTransform){
        this.targetTransform = targetTransform;
    }

    private void LateUpdate(){
        if(targetTransform == null) return;
        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}
