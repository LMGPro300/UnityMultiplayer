using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour//MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxAccelAir, jumpSpeed, accelGround, friction, maxSpeed, airControl, gravity, accelAir;
    [SerializeField] private PlayerCollision playerCollision;
    [SerializeField] private ClientPrediction clientPrediction;

    public Vector3 wishDirection;
    private NetworkTimer networkTimer;
    private float jumpInput;


    public void Awake(){
        Debug.Log("am awaken");

    }
    

    private void Gravity(){
        if(playerCollision.GetIsGrounded() == false){
            rb.velocity += new Vector3(0,-gravity*Time.deltaTime,0);
        }
    }

    private Vector3 Friction(Vector3 finalVelo){
        float speed = finalVelo.magnitude;
        float initY = finalVelo.y;
        if (speed != 0){
            float drop = speed * friction * Time.deltaTime;
            finalVelo *= Mathf.Max(speed - drop, 0) / speed; 
        }
        finalVelo.y = initY;
        return finalVelo;
    }

     private Vector3 Accelerate(Vector3 wishDir, float acceleration){
        //Debug.Log("Accelerating ground");
        //Debug.Log(rb.velocity.y == 0f);
        Vector3 finalVelo = Friction(rb.velocity);
        float projVel = Vector3.Dot(finalVelo, wishDir);
        float addSpeed = maxSpeed - projVel;
        float accelVel = acceleration * Time.deltaTime; 
        if (accelVel > addSpeed)
            accelVel = addSpeed;
        return finalVelo +  wishDir * accelVel;
    }

    private Vector3 Accelerate(Vector3 wishDir, float acceleration, float airControl, float maxAccel){
        //Debug.Log("Accelerating air");
        //Debug.Log(rb.velocity.y == 0f);
        float projVel = Vector3.Dot(rb.velocity, wishDir);
        float addSpeed = maxAccel - projVel;
        //if (Mathf.Round(addSpeed) == 0) {//|| Mathf.Round(addSpeed) == maxAccel - maxSpeed){
            //Debug.Log("removing limiter");
           // return Accelerate(wishDir, acceleration, airControl, maxSpeed);
        //}
        
        float accelVel = acceleration *  airControl * Time.deltaTime; //* airControl 
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

    void FixedUpdate(){
        if (!IsOwner) return;

        while (clientPrediction.networkTimer.ShouldTick()){
            clientPrediction.HandleClientTick();
            clientPrediction.HandleServerTick();
        }
    }

    


    public void Move(){
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

    void respawn(){
        if (transform.position.y < - 200){
            transform.position = new Vector3(0, 5, 0);
        }
    }

    public void Jump(float jumpInput){
        if (jumpInput == 1 && playerCollision.GetIsGrounded()){
            Debug.Log((jumpSpeed * Time.deltaTime) + " jump speed at one point");
            rb.velocity = new Vector3 (rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            playerCollision.DidJump();
        }
    }

    public void RecieveJumpInput(float jumpInput){
        //Debug.Log(jumpInput == 1);
        //Debug.Log(playerCollision.GetIsGrounded());
        this.jumpInput = jumpInput;
    }

    public void RecieveKeyboardInput(Vector2 keyboardInput){
        wishDirection.x = keyboardInput.x;
        wishDirection.z = keyboardInput.y;
        wishDirection = transform.TransformDirection(wishDirection);
    }

    public Vector3 getWishDir(){
        return wishDirection;
    }


    
}


