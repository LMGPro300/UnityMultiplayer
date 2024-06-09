using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour{
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

    public void RecieveShootInput(float input){
        if (shootingController.canShoot(input) && isGun){
            //shootingAnimation.Play("Armature|Shoot");
            armAnimator.Play(null);
            armAnimator.Play(shootingAnimation);
            shootingController.Shoot(cam.transform.position, cam.transform.forward);
        }
    }

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

    public void ChangeSlot(GunScriptableObject gunScriptableObject){
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

        shootingController.ChangeGun(gunScriptableObject);

        armAnimator.Play(null);
        armAnimator.Play(equipAnimation);
        if (itemAnimator != null){
            itemAnimator.Play(null);
            itemAnimator.Play(gunEquipAnimation);
        }
    }

    public void GetItemAnimator(Animator itemAnimator){
        this.itemAnimator = itemAnimator;
    }

}
