using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/*
 * Program name: PlayerShoot.cs
 * Author: Elvin Shen 
 * What the program does: Handles shooting interactions with the player
 */

public class PlayerShoot : NetworkBehaviour{
    [SerializeField] private Transform cam;
    [SerializeField] private ShootingController shootingController;
    [SerializeField] private Animator armAnimator;
    private Animator itemAnimator;
    private bool isGun = false;
    private string shootingAnimation;
    private string reloadAnimation;
    private string gunReloadAnimation;
    private string equipAnimation;
    private string gunEquipAnimation;
    //[SerializeField] private Animator shootingAnimation;

    //Recieve a shoot input, if it can shoot, play the animation and shoot
    public void RecieveShootInput(float input){
        if (shootingController.canShoot(input) && isGun){
            //shootingAnimation.Play("Armature|Shoot");
            armAnimator.Play(null);
            armAnimator.Play(shootingAnimation);
            shootingController.Shoot(cam.transform.position, cam.transform.forward);
        }
    }

    //Recieve a reload input, if it can reload, play the animation and reload
    public void RecieveReloadInput(float reloadInput){
        if (shootingController.canReload(reloadInput) && isGun){
            shootingController.ReloadMag();
            armAnimator.Play(null);
            armAnimator.Play(reloadAnimation);
            if (itemAnimator != null){
                itemAnimator.Play(null);
                itemAnimator.Play(gunReloadAnimation);
            }
        }
    }

    //The player has switch weapons, readjust the data to match the new weapon
    public void ChangeSlot(GunScriptableObjectWrapper gunScriptableObject){
        //the weapon switched is not a gun
        if (gunScriptableObject == null){
            isGun = false;
            return;
        }
        isGun = true;
        shootingAnimation = gunScriptableObject.shootingAnimation;
        reloadAnimation = gunScriptableObject.reloadAnimation;
        gunReloadAnimation = gunScriptableObject.gunReloadAnimation;
        equipAnimation = gunScriptableObject.equipAnimation;
        gunEquipAnimation = gunScriptableObject.gunEquipAnimation;

        //Play the equip animations

        shootingController.ChangeGun(gunScriptableObject);

        armAnimator.Play(null);
        armAnimator.Play(equipAnimation);
        if (itemAnimator != null){
            itemAnimator.Play(null);
            itemAnimator.Play(gunEquipAnimation);
        }
    }

    //Get the animator from the item the player spawned in
    public void GetItemAnimator(Animator itemAnimator){
        this.itemAnimator = itemAnimator;
    }

}
