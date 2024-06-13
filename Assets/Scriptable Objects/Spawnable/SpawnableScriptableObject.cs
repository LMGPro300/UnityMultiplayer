using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "All Spawnable", menuName = "All Spawnable/All Spawnable")]
public class SpawnableScriptableObject : ScriptableObject{
    [Header("Gun Spawns")]
    public int gunChance = 10;
    public GameObject[] guns; 
    private int numGuns;

    [Header("Melee")]
    public int meleeChance = 20;
    public GameObject[] melees;
    private int numMelees;

    [Header("Item Spawns")]
    public int itemChance = 30;
    public GameObject[] items;
    private int numItems;

    [Header("Garbage")]
    public int garbageChance = 40;
    public GameObject[] garbages;
    private int numGarbage;


    public int[] totalChances;

    public void Init(){
        numGuns = guns.Length;
        numItems = items.Length;
        numGarbage = garbages.Length;
        numMelees = melees.Length;
        totalChances = new int[5] {gunChance, meleeChance, itemChance, garbageChance, 1000000};
    }

    public GameObject RandomGun(){
        return guns[Random.Range(0, numGuns)];
    }
    public GameObject RandomItem(){
        return items[Random.Range(0, numItems)];
    }
    public GameObject RandomGarbage(){
        return garbages[Random.Range(0, numGarbage)];
    }
    public GameObject RandomMelee(){
        return melees[Random.Range(0, numMelees)];
    }
}



