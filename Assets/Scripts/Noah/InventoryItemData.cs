using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(menuName = "item data")]
public class InventoryItemData : MonoBehaviour
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab;
}
