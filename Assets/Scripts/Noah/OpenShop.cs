using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenShop : MonoBehaviour
{
    [SerializeField]
    private InputAction openShop;
    [SerializeField]
    private TextMeshProUGUI canOpenShop;
    [SerializeField]
    private ShopManager shopManager;

    bool canDisplayShop = false;
    bool shopIsDisplaying = false;

    public void OnEnable()
    {
        openShop.Enable();
        openShop.performed += DisplayShop;
    }

    public void OnDisable()
    {
        openShop.Disable();
        openShop.performed -= DisplayShop;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy stop zone") {
            canDisplayShop = true;
            canOpenShop.text = "Press G to Open Shop";
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy stop zone")
        {
            canDisplayShop = false;
            canOpenShop.text = "";
        }
    }

    public void DisplayShop(InputAction.CallbackContext ctx)
    {
        if (canDisplayShop)
        {
            if (canOpenShop.text == "")
            {
                canOpenShop.text = "Press G to Open Shop";
            }
            else
            {
                canOpenShop.text = "";
            }
            shopManager.displayShop();
        }
    }

}
