using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    [SerializeField]
    [Header("buttons")]
    public Button hello;

    private void Awake()
    {
        hello.onClick.AddListener(hellothere);
    }

    public void hellothere()
    {
        Debug.Log("hello there");
    }
}
