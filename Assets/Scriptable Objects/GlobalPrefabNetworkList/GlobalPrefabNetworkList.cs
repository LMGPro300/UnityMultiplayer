using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalPrefabNetworkList", menuName = "Networking/GlobalPrefabNetworkList")]
public class GlobalPrefabNetworkList : ScriptableObject
{
    public List<GameObject> _globalPrefabNetworkList;
}
