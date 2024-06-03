using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<MyTabButton> tabButtons;
    public List<GameObject> objectsToSwap;

    public int index = 0;

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

    public int GetTabIndex()
    {
        return index;
    }

    public GameObject GetCurrentPage()
    {
        return objectsToSwap[index];
    }
}
