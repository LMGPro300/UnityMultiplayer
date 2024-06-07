using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour{
    [SerializeField] private Transform cam;
    [SerializeField] private ShootingController shootingController;
    //[SerializeField] private Animator shootingAnimation;

    public void RecieveShootInput(float input){
        if (shootingController.canShoot(input)){
            //shootingAnimation.Play("Armature|Shoot");
            shootingController.Shoot(cam.transform.position, cam.transform.forward);
        }
    }
}
