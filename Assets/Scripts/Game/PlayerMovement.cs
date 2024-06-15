using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/*
 * Program name: PlayerMovement.cs
 * Author: Elvin Shen
 * What the program does: Calculates all WASD input from the user into movement
 * Credits: https://adrianb.io/2015/02/14/bunnyhop.html
 */

public class PlayerMovement : NetworkBehaviour//MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxAccelAir, jumpSpeed, accelGround, friction, maxSpeed, airControl, gravity, accelAir, frameRate;
    [SerializeField] private PlayerCollision playerCollision;
    [SerializeField] private PlayerPrediction playerPrediction;
    [SerializeField] private InventorySystem inventorySystem;
    [SerializeField] private Canvas playerUI;
    public Vector3 wishDirection;
    private float jumpInput;
    [SerializeField] private float networkDeltaTime;


    //When spawned, only toggle your own inventory and user interface
    //also define the framerate it's going to be running at
    public override void OnNetworkSpawn(){
        base.OnNetworkSpawn();
        if (!IsOwner) return; 
        inventorySystem.enabled = true;
        playerUI.enabled = true;
        //transform.position = new Vector3(0f, 10f, 0f);
        Debug.Log("it worked SPANWI GBFY8fisadf");
        networkDeltaTime = 1f/frameRate;
        playerPrediction.SetNewThresHold(100000f);
        Debug.Log("it's spawning");
        SceneManager.sceneLoaded += loadComplete;

        //NetworkSceneManager.OnLoadCompleteDelegateHandler += () => {OnLoadComplete();};

        //playerPrediction.enabled = true;
        //transform.rotation = Quaternion.Euler(-90f, 0f, 0f) * transform.rotation;
    }

    //When the player loads, spawn it into one of the random spawnpoints
    public void loadComplete(Scene scene, LoadSceneMode mode){
        Debug.Log("changed");
        transform.position = SpawnPoints.Instance.RandomSpawnPoint();
        PlayerManager.Instance.AddPlayer(transform);
    }
    
    //Apply gravity if the player isnt grounded
    private void Gravity(){
        if(playerCollision.GetIsGrounded() == false){
            rb.velocity += new Vector3(0,-gravity*networkDeltaTime,0);
        }
    }

    //Apply friction by scaling the velocity based on friction.
    private Vector3 Friction(Vector3 finalVelo){
        float speed = finalVelo.magnitude;
        float initY = finalVelo.y;
        if (speed != 0){
            float drop = speed * friction * networkDeltaTime;
            finalVelo *= Mathf.Max(speed - drop, 0) / speed; 
        }
        finalVelo.y = initY;
        return finalVelo;
    }

    //Wishdir is the normalized vector the player wants to move towards 
    //and accounted for player rotation
    //if the dot product of the direction it's moving in is surpassing the maxSpeed
    //limit the velocity
     private Vector3 Accelerate(Vector3 wishDir, float acceleration){
        //Debug.Log("Accelerating ground");
        //Debug.Log(rb.velocity.y == 0f);
        Vector3 finalVelo = Friction(rb.velocity);
        float projVel = Vector3.Dot(finalVelo, wishDir);
        float addSpeed = maxSpeed - projVel;
        float accelVel = acceleration * networkDeltaTime; 
        if (accelVel > addSpeed)
            accelVel = addSpeed;
        return finalVelo +  wishDir * accelVel;
    }

    //Same acceleration but without friction and uses slightly different
    //modifications to air movement
    private Vector3 Accelerate(Vector3 wishDir, float acceleration, float airControl, float maxAccel){
        //Debug.Log("Accelerating air");
        //Debug.Log(rb.velocity.y == 0f);
        float projVel = Vector3.Dot(rb.velocity, wishDir);
        float addSpeed = maxAccel - projVel;
        //if (Mathf.Round(addSpeed) == 0) {//|| Mathf.Round(addSpeed) == maxAccel - maxSpeed){
            //Debug.Log("removing limiter");
           // return Accelerate(wishDir, acceleration, airControl, maxSpeed);
        //}
        
        float accelVel = acceleration *  airControl * networkDeltaTime; //* airControl 
        if (accelVel > addSpeed)

            accelVel = maxAccel- projVel;
        //if(projVel + accelVel > maxSpeed){
            //accelVel = maxAccel - projVel;
        //}
        //if(projVel + accelVel > maxAccel){
        //    accelVel = maxAccel - projVel;
        //}
        //Debug.Log(wishDir * accelVel + " amount velo added / restricted");
        //Debug.Log(accelVel);
        return rb.velocity +  wishDir * accelVel;
    }

    //Move the player in the wish direction and the jumpInput;
    public void Move(Vector3 wishDirection, float jumpInput){
        Vector3 finalVelo = new Vector3();
        //ebug.Log(playerCollision.GetIsGrounded());
        if(playerCollision.GetIsAir()){ 
            finalVelo = Accelerate(wishDirection, accelGround);
        } else{
            //Debug.Log("velo1 " + rb.velocity);
            finalVelo = Accelerate(wishDirection, accelAir, airControl, maxAccelAir);
           // Debug.Log("velo2 " + rb.velocity);
        }
        rb.velocity = new Vector3(finalVelo.x, rb.velocity.y, finalVelo.z);
        rb.angularVelocity = Vector3.zero;

        respawn();
        Jump(jumpInput);
        Gravity();
    }

    //if the player somehow just falls beyond the map
    void respawn(){
        if (transform.position.y < - 200){
            transform.position = SpawnPoints.Instance.RandomSpawnPoint();
            PlayerManager.Instance.AddPlayer(transform);
        }
    }

    //Apply a force upwards when jumping on the ground
    public void Jump(float jumpInput){
        if (jumpInput == 1 && playerCollision.GetIsGrounded()){
            Debug.Log((jumpSpeed * networkDeltaTime) + " jump speed at one point");
            rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            playerCollision.DidJump();
        }
    }

    //gotten from input handler, passes the variable on
    public void RecieveJumpInput(float jumpInput){
        //Debug.Log(jumpInput == 1);
        //Debug.Log(playerCollision.GetIsGrounded());
        this.jumpInput = jumpInput;
    }

    //keys from input handler, and tranformed into world space
    public void RecieveKeyboardInput(Vector2 keyboardInput){
        wishDirection.x = keyboardInput.x;
        wishDirection.z = keyboardInput.y;
        wishDirection = transform.TransformDirection(wishDirection);
    }

    public Vector3 getWishDir(){
        return wishDirection;
    }

    public float getJumpInput(){
        return this.jumpInput;
    }

    public bool IsMoving(){
        return rb.velocity.x + rb.velocity.z > 0.1;
    }


    
}


