using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/New Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public int health = 100;
}
