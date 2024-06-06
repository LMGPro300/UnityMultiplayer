using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ShopManager : MonoBehaviour
{ 
    public InventorySystem inventory;
    public int playerCredits = 100;
    public TextMeshProUGUI playerBalanceText;
    public TextMeshProUGUI shopErrorText;
    public InputAction openShop;
    public GameObject shopUI;
    public int errorPopupTime = 2;

    private CountdownTimer timer;

    public void Awake()
    {
        playerBalanceText.text = "Current Balance: " + playerCredits;
        openShop.Enable();
        openShop.performed += displayShop;
        timer = new CountdownTimer(errorPopupTime);
        timer.OnTimerStop += RemoveErrorMessage;
    }

    public void OnDisable()
    {
        openShop.performed -= displayShop;
        openShop.Disable();
    }

    public void displayShop(InputAction.CallbackContext ctx)
    {
        shopUI.SetActive(!shopUI.activeSelf);
    }
    void Update()
    {
        timer.Tick(Time.deltaTime);
    }

    public void itemClicked(ShopItemData clickedObject)
    {
        if (!inventory.canAddItem(clickedObject.inventory_item))
        {
            timer.Start();
            shopErrorText.text = "Your inventory is full";
        }
        else if (playerCredits - clickedObject.price < 0)
        {
            timer.Start();
            shopErrorText.text = "You don't have enough credits";
        }
        else
        {
            playerCredits -= clickedObject.price;
            playerBalanceText.text = "Current Balance: " + playerCredits;
            inventory.Add(clickedObject.inventory_item);
        }
    }

    public void RemoveErrorMessage()
    {
        shopErrorText.text = "";
    }
}
