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

    public void itemClicked(ShopItem clickedObject)
    {
        if (clickedObject.quantity <= 0)
        {
            timer.Start();
            shopErrorText.text = "The item is sold out";
        }
        else if (!inventory.canAddItem(clickedObject.data.inventory_item))
        {
            timer.Start();
            shopErrorText.text = "Your inventory is full";
        }
        else if (playerCredits - clickedObject.data.price < 0)
        {
            timer.Start();
            shopErrorText.text = "You don't have enough credits";
        }
        else
        {
            playerCredits -= clickedObject.data.price;
            playerBalanceText.text = "Current Balance: " + playerCredits;

            clickedObject.RemoveFromStack();
            inventory.Add(clickedObject.data.inventory_item);
        }
    }

    public void RemoveErrorMessage()
    {
        shopErrorText.text = "";
    }
}
