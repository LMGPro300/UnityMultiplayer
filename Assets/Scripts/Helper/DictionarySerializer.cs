using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


[Serializable]
public class DictionarySerializer
{
    [SerializeField] NewDictItem[] Data;
    private Dictionary<string, float> myDict = new Dictionary<string, float>();

    public Dictionary<string, float> GetDictionary(){
        foreach (NewDictItem item in Data){
            myDict.Add(item.name, item.value);
        }
        return myDict;
    }
}

[Serializable]
public class NewDictItem
{
    public string name;
    public float value;
}