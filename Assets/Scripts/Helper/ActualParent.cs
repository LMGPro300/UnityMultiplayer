using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Program name: ActualParent.cs
 * Author: Elvin Shen
 * What the program does: Points to the actual parent of a gameobject, sometimes colliders get weird
 */
public class ActualParent : MonoBehaviour
{
    [SerializeField] public GameObject actualParent;
}
