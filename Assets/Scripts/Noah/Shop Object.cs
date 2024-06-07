using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopObject : MonoBehaviour, IPointerClickHandler
{
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

    public void OnPointerClick(PointerEventData eventData)
    {
        shopManager.itemClicked(referenceData);
        UpdateShopItemUI();
    }

    public void UpdateShopItemUI()
    {
        itemNameText.text = referenceData.shopItemName;
        itemPriceText.text = referenceData.price + " credits";
        itemIconImage.sprite = referenceData.icon;
    }
}
