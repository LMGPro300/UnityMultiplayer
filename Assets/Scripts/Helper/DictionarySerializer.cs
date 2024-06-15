using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
/*
 * Program name: DictionarySerializer.cs
 * Author: Elvin Shen
 * What the program does: Allows Unity to serialize Dictionaries (sort of) for other scripts
 */

[Serializable]
public class DictionarySerializer
{
    [SerializeField] NewDictItem[] Data;
    private Dictionary<string, float> myDict = new Dictionary<string, float>();

    public Dictionary<string, float> GetDictionary(){
        if (myDict.Count != 0){
            return myDict;    
        }
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