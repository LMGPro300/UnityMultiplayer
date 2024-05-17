using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float maxAccelAir, jumpSpeed, accelGround, friction, maxSpeed, airControl, gravity, accelAir;
    [SerializeField] private PlayerCollision playerCollision;
    private Vector3 wishDirection;
    

    private void Gravity(){
        if(playerCollision.GetIsGrounded() == false){
            rb.velocity += new Vector3(0,-gravity*Time.fixedDeltaTime,0);
        }
    }

    private Vector3 Friction(Vector3 finalVelo){
        float speed = finalVelo.magnitude;
        float initY = finalVelo.y;
        if (speed != 0){
            float drop = speed * friction * Time.fixedDeltaTime;
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
        float accelVel = acceleration * Time.fixedDeltaTime; 
        if (accelVel > addSpeed)
            accelVel = addSpeed;
        return finalVelo +  wishDir * accelVel;
    }

    private Vector3 Accelerate(Vector3 wishDir, float acceleration, float airControl, float maxAccel){
        //Debug.Log("Accelerating air");
        //Debug.Log(rb.velocity.y == 0f);
        float projVel = Vector3.Dot(rb.velocity, wishDir);
        float addSpeed = maxAccel - projVel;
        float accelVel = acceleration * Time.fixedDeltaTime* airControl; //* airControl
        //if (accelVel > addSpeed)
        //    accelVel = addSpeed;
        //if(projVel + accelVel > maxAccel){
        //    accelVel = maxAccel - projVel;
        //}
        if(projVel + accelVel > maxAccel){
            accelVel = maxAccel - projVel;
        }
        //Debug.Log(wishDir * accelVel + " amount velo added / restricted");
        return rb.velocity +  wishDir * accelVel;
    }

    void FixedUpdate(){
        Vector3 finalVelo = new Vector3();
        //ebug.Log(playerCollision.GetIsGrounded());
        if(playerCollision.GetIsAir()){ 
            finalVelo = Accelerate(wishDirection, accelGround);
        } else{
            finalVelo = Accelerate(wishDirection, accelAir, airControl, maxAccelAir);
        }
        rb.velocity = new Vector3(finalVelo.x, rb.velocity.y, finalVelo.z);
        rb.angularVelocity = Vector3.zero;

        respawn();
        Gravity();
    }

    void respawn(){
        if (transform.position.y < - 200){
            transform.position = new Vector3(0, 5, 0);
        }
    }

    

    public void RecieveJumpInput(float jumpInput){
        //Debug.Log(jumpInput == 1);
        //Debug.Log(playerCollision.GetIsGrounded());
        if (jumpInput == 1 && playerCollision.GetIsGrounded()){
            rb.AddForce(Vector3.up * jumpSpeed * Time.fixedDeltaTime, ForceMode.Impulse);
        }
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


