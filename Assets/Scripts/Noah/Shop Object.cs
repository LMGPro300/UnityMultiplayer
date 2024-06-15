using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
 * Program name: ShopObject.cs
 * Author: Noah Levy, some help from tutorial
 * What the program does: this is on each button object in the shop, handles buying items
 */

//TUTORIAL USED: https://www.youtube.com/watch?v=211t6r12XPQ

public class ShopObject : MonoBehaviour, IPointerClickHandler
{
    //get reference to shop manager and other important UI data
    [SerializeField]
    private ShopItemData referenceData;
    [SerializeField]
    private ShopManager shopManager;
    [SerializeField]
    private TextMeshProUGUI itemNameText;
    [SerializeField]
    private TextMeshProUGUI itemPriceText;
    [SerializeField]
    private Image itemIconImage;

    public void Awake()
    {
        UpdateShopItemUI();
    }

    //pass in clicked data to the shopManager
    public void OnPointerClick(PointerEventData eventData)
    {
        shopManager.itemClicked(referenceData);
        UpdateShopItemUI();
    }

    //update shop UI with current prices and player balance
    public void UpdateShopItemUI()
    {
        itemNameText.text = referenceData.shopItemName;
        itemPriceText.text = referenceData.price + " credits";
        itemIconImage.sprite = referenceData.icon;
    }
}
