using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabNetworkList", menuName = "Networking/PrefabNetworkList")]
public class PrefabNetworkList : ScriptableObject
{
    public List<GameObject> _prefabNetworkList;
}
