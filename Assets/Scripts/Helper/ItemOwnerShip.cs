using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOwnerShip : MonoBehaviour{
    private ShopManager shopManager;

    public ShopManager Owner(){
        return shopManager;
    }
}
