using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicItemData
{
    public int ammo = 10;
    public ShopManager pastOwner = null;
    public DynamicItemData() {}

    public void SetPastShopOwner(ShopManager pO) {
        this.pastOwner = pO;
    }

    public void SetAmmoCount(int count)
    {
        this.ammo = count;
    }
}
