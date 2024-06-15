using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;
using Unity.Netcode;
/*
 * Program name: ShootingController.cs
 * Author: Elvin Shen
 * What the program does: Controls the raycasts and the information of the raycast
 */

public class ShootingController: NetworkBehaviour
{
    private GunScriptableObject gunScriptableObject;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private LayerMask ignoreLayer;
    //[SerializeField] private VisualEffect blood;
    //[SerializeField] private VisualEffect fount;
    //[SerializeField] private VisualEffect flashs;
    public int smallAmmo = 0;
    public int mediumAmmo = 30;
    public int largeAmmo = 0;
   // [SerializeField] private Transform bulletSpawnPoint;

    //[SerializeField] private TrailRenderer bulletTrail;
    private int currentAmmoCount;
    private AmmoType currentAmmoType;
    private GunScriptableObjectWrapper currentGunObject;
    //[SerializeField] private AudioSource audioSource;

    private int magSize;
    private int currentMagSize;
    //private int ammoCapacity;
    private float damage;
    private float force;
    private float firerate;
    private float reloadTime;

    private Dictionary<string, float> entityComponents;
    private CountdownTimer shootTimer;
    private CountdownTimer reloadTimer;


    //Define the timers of firerate and reloading timer
    public void Awake(){
        shootTimer = new CountdownTimer(firerate);
        reloadTimer = new CountdownTimer(reloadTime);
    }

    //Find the correct ammo to display as reserve ammo
    public void CorrectAmmoType(AmmoType ammoType){
        if (ammoType == AmmoType.smallAmmo){
            currentAmmoCount = smallAmmo;
        } else if (ammoType == AmmoType.mediumAmmo){
            currentAmmoCount = mediumAmmo;
        } else if (ammoType == AmmoType.largeAmmo){
            currentAmmoCount = largeAmmo;
        }
    }

    //Decrease the correct ammo type
    public void DecreaseAmmoType(int bullets){
        if (currentAmmoType == AmmoType.smallAmmo){
            smallAmmo -= bullets;
        } else if (currentAmmoType == AmmoType.mediumAmmo){
            mediumAmmo -= bullets;
        } else if (currentAmmoType == AmmoType.largeAmmo){
            largeAmmo -= bullets;
        }
    }

    //Update the ammo counter
    public void UpdateAmmoText(){
        ammoText.text = "Ammo: " + currentMagSize + "/" + currentAmmoCount;
        currentGunObject.currentMagSize = currentMagSize;
    }
    
    //Gun was changed, switch the data accordingly
    public void ChangeGun(GunScriptableObjectWrapper gunScriptableObject){
        currentGunObject = gunScriptableObject;
        magSize = gunScriptableObject.magSize;
        currentMagSize = gunScriptableObject.currentMagSize;
        reloadTime = gunScriptableObject.reloadTime;
        //ammoCapacity = gunScriptableObject.ammoCapacity;
        damage = gunScriptableObject.damage;
        firerate = gunScriptableObject.firerate;
        force = gunScriptableObject.force;
        currentAmmoType = gunScriptableObject.ammoType;
        CorrectAmmoType(currentAmmoType);
        entityComponents = gunScriptableObject.EntityComponents;
        shootTimer.SetNewTime(firerate);
        reloadTimer.SetNewTime(reloadTime);
        shootTimer.Start();
        UpdateAmmoText();
    }

    public void Update(){
        shootTimer.Tick(Time.deltaTime);
        reloadTimer.Tick(Time.deltaTime);
    }

    //Start the firerate timer, lost one bullet, and shoot a raycast to see if it hit anything
    public void Shoot(Vector3 starting, Vector3 target){
        shootTimer.Start();
        currentMagSize -= 1;
        UpdateAmmoText();

        RaycastHit hitInfo;
        bool maybeHit = Physics.Raycast(starting, target, out hitInfo, 5000, ~ignoreLayer);
        Debug.Log("lowkey I missed u you suck LMAOoo " + maybeHit);

        //If it hit something, call IShootable and damage the entity
        if (maybeHit){ //int = range
            IShootAble shootable = hitInfo.collider.GetComponent<IShootAble>();
            if (shootable == null){
                Debug.Log("bruh I hit nothing :(");
                return;
            }
            //if the entity died, call it's death function
            handleShot(shootable);
            if (shootable.isAlive() == false){
                handleDeath(target, shootable, hitInfo.point);
            }
        }
    }


    //Calculate damage dealt, body parts such as headshot do more damage
    //leg and arm shots do less
    public void handleShot(IShootAble shootable){
        float damageDealt = damage;
        string hitType = shootable.hitType();
        if (entityComponents.ContainsKey(hitType)){
            damageDealt *= entityComponents[hitType];
        }
        shootable.IsShot(damageDealt);  
        
    }

    //Apply the ragdoll when died
    public void handleDeath(Vector3 targetRay, IShootAble shootable, Vector3 hitLocation){
        targetRay.Normalize();
        Vector3 forceApplied = targetRay * force;
        shootable.DoRagdoll(forceApplied, hitLocation);
    }

    //Reload the magazine and calculate how many bullets are needed to get a full mag
    public void ReloadMag(){
        if (currentAmmoCount != 0){
            reloadTimer.Start();
            int diff = magSize - currentMagSize;
            currentMagSize = diff > currentAmmoCount ? currentMagSize + currentAmmoCount : magSize;
            if(currentAmmoCount < diff){
                DecreaseAmmoType(currentAmmoCount);
            } else {
                DecreaseAmmoType(diff);
            }
            currentAmmoCount = currentAmmoCount < diff ? 0 : currentAmmoCount - diff;
            UpdateAmmoText();
        }
    }

    public bool canReload(float input){
        return input == 1 && currentAmmoCount != 0 && reloadTimer.IsFinished() && magSize != currentMagSize;
    }

    public bool canShoot(float input){
        return input == 1 && shootTimer.IsFinished() && currentMagSize != 0 && reloadTimer.IsFinished();
    }
}
