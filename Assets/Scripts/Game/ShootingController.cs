using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using TMPro;


public class ShootingController: MonoBehaviour
{
    private GunScriptableObject gunScriptableObject;
    [SerializeField] private TextMeshProUGUI ammoText;
    //[SerializeField] private VisualEffect blood;
    //[SerializeField] private VisualEffect fount;
    //[SerializeField] private VisualEffect flashs;
    private int smallAmmo = 0;
    private int mediumAmmo = 30;
    private int largeAmmo = 0;
   // [SerializeField] private Transform bulletSpawnPoint;

    //[SerializeField] private TrailRenderer bulletTrail;
    private int currentAmmoCount;
    private AmmoType currentAmmoType;
    private GunScriptableObject currentGunObject;
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

    public void Awake(){
        shootTimer = new CountdownTimer(firerate);
        reloadTimer = new CountdownTimer(reloadTime);
    }

    public void CorrectAmmoType(AmmoType ammoType){
        if (ammoType == AmmoType.smallAmmo){
            currentAmmoCount = smallAmmo;
        } else if (ammoType == AmmoType.mediumAmmo){
            currentAmmoCount = mediumAmmo;
        } else if (ammoType == AmmoType.largeAmmo){
            currentAmmoCount = largeAmmo;
        }
    }

    public void DecreaseAmmoType(int bullets){
        if (currentAmmoType == AmmoType.smallAmmo){
            smallAmmo -= bullets;
        } else if (currentAmmoType == AmmoType.mediumAmmo){
            mediumAmmo -= bullets;
        } else if (currentAmmoType == AmmoType.largeAmmo){
            largeAmmo -= bullets;
        }
    }

    public void UpdateAmmoText(){
        ammoText.text = "Ammo: " + currentMagSize + "/" + currentAmmoCount;
        currentGunObject.currentMagSize = currentMagSize;
    }
    
    public void ChangeGun(GunScriptableObject gunScriptableObject){
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
        entityComponents = gunScriptableObject.EntityComponents.GetDictionary();
        shootTimer.SetNewTime(firerate);
        reloadTimer.SetNewTime(reloadTime);
        shootTimer.Start();
        UpdateAmmoText();
    }

    public void Update(){
        shootTimer.Tick(Time.deltaTime);
        reloadTimer.Tick(Time.deltaTime);
    }
    public void Shoot(Vector3 starting, Vector3 target){
        shootTimer.Start();
        currentMagSize -= 1;
        UpdateAmmoText();

        RaycastHit hitInfo;
        bool maybeHit = Physics.Raycast(starting, target, out hitInfo, 5000);

        //showTracer(hitInfo);
        //showFlash();
        //audioSource.Play();
        
        if (maybeHit){ //int = range
            IShootAble shootable = hitInfo.collider.GetComponent<IShootAble>();
            if (shootable == null){
                return;
            }
            handleShot(shootable);
            //showBlood(target, hitInfo);
            if (shootable.isAlive() == false){
                handleDeath(target, shootable, hitInfo.point);
            }
        }
    }

    /*

    public void showBlood(Vector3 target, RaycastHit hitInfo){
        target.Normalize();
        Vector3 bloodTrail = target*(Random.Range(5, 15));
        bloodTrail.y = 1;
        float distance = -2.6f;
        Ray ray = new Ray(hitInfo.transform.position, -Vector3.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)){
            if (hit.collider.tag == "Ground"){
                distance = hit.distance;
            }
        }

        VisualEffect effect = Instantiate(blood, hitInfo.collider.transform.position, Quaternion.Euler(0f, 0f, 0f));
        VisualEffect effect1 = Instantiate(fount, hitInfo.collider.transform.position, Quaternion.Euler(0f, 0f, 0f));
        effect1.SetFloat("Height", (-distance) -0.5f);
        effect1.SetBool("Loop", false);
        effect1.SetInt("LifeTime", 30);
        effect.SetBool("Loop", false);
        effect.SetInt("LifeTime", 30);
        effect.SetVector3("Velocity", bloodTrail);
        effect.SetFloat("Height", (-distance) -0.5f);
        effect.Play();
        effect1.Play();


        Destroy(effect, 40f);
        Destroy(effect1, 40f);
    }

    */

    /*
    public void showTracer(RaycastHit hitInfo){
        TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, hitInfo));
    }

    public void showFlash(){

        flashs.Play();
        //VisualEffect effect2 = Instantiate(flashs);
        //effect2.Play();

        //Destroy(effect2, 1f);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hitInfo){
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 1){
            trail.transform.position = Vector3.Lerp(startPos, hitInfo.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hitInfo.point;
        //Instantiate

        Destroy(trail.gameObject, trail.time);
    }
    */

    public void handleShot(IShootAble shootable){
        float damageDealt = damage;
        string hitType = shootable.hitType();
        if (entityComponents.ContainsKey(hitType)){
            damageDealt *= entityComponents[hitType];
        }
        shootable.IsShot(damageDealt);  
        
    }

    public void handleDeath(Vector3 targetRay, IShootAble shootable, Vector3 hitLocation){
        targetRay.Normalize();
        Vector3 forceApplied = targetRay * force;
        shootable.DoRagdoll(forceApplied, hitLocation);
    }

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
