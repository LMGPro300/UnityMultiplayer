using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopObject : MonoBehaviour, IPointerClickHandler
{
    public ShopItemData referenceData;
    public ShopManager shopManager;

    private ShopItem myItem;

    public void Awake()
    {
        myItem = new ShopItem(referenceData);
        UpdateShopItemUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        shopManager.itemClicked(myItem);
        UpdateShopItemUI();
    }

    public void UpdateShopItemUI()
    {
        transform.Find("Item Name Text").GetComponent<TextMeshProUGUI>().text = myItem.data.shopItemName;
        transform.Find("Item Price Text").GetComponent<TextMeshProUGUI>().text = myItem.data.price + " credits";
        transform.Find("QuantityBackgroundImage").gameObject.transform.Find("Quantity Text").gameObject.GetComponent<TextMeshProUGUI>().text = myItem.quantity + "";
        transform.Find("Icon Image").gameObject.GetComponent<Image>().sprite = myItem.data.icon;
    }
}
