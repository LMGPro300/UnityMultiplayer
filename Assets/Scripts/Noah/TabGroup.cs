using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Program name: TabGroup.cs
 * Author: Noah Levy, some code taken from tutorial
 * What the program does: allows tab scripts to subscribe to this one
 */

// SOME CODE TAKEN FROM THIS TUTORIAL: https://www.youtube.com/watch?v=211t6r12XPQ

public class TabGroup : MonoBehaviour
{
    //define a list of known tab objects and item pages
    public List<MyTabButton> tabButtons;
    public List<GameObject> objectsToSwap;

    //current tab we are at
    public int index = 0;

    //adds tabs from shop to this script
    public void Subscribe(MyTabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<MyTabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(MyTabButton button)
    {

    }

    public void OnTabExit(MyTabButton button)
    {

    }

    //handles setting pages active and not active when switching tabs
    public void OnTabSelected(MyTabButton button)
    {
        index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    //helpful getter methods for current tab and page object
    public int GetTabIndex()
    {
        return index;
    }

    public GameObject GetCurrentPage()
    {
        return objectsToSwap[index];
    }
}
