using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement1 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public InputAction movement;
    [SerializeField]
    public InputAction looking;
    [SerializeField]
    public InputAction jump;
    [SerializeField]
    public float playerSpeed = 20f, mouseSens = 0f;
    [SerializeField]
    public Camera playerCam;

    bool isGrounded = true;

    Rigidbody rb;
    Vector2 strafeInput;
    Vector2 mouseInput;
    Vector3 playerVelocity;
    float camAngle;

    void Start()
    {
        movement.Enable();
        looking.Enable();
        jump.Enable();
        jump.performed += playerJump;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = looking.ReadValue<Vector2>();
        strafeInput = movement.ReadValue<Vector2>();
        playerVelocity = new Vector3(strafeInput.x, 0f, strafeInput.y);
        playerVelocity = transform.TransformDirection(playerVelocity);
    }
    
    private void FixedUpdate()
    {
        //transform.Rotate(0f, mouseSens*mouseInput.y*Time.fixedDeltaTime, 0f);
        /*rb.velocity = new Vector3(playerVelocity.x * playerSpeed * Time.fixedDeltaTime, rb.velocity.y, playerVelocity.y * playerSpeed * Time.fixedDeltaTime);*/
        float pastGrav = rb.velocity.y;
        rb.velocity = playerVelocity * Time.fixedDeltaTime * playerSpeed;
        rb.velocity = new Vector3(rb.velocity.x, pastGrav, rb.velocity.z);
        transform.rotation *= Quaternion.Euler(0f, mouseInput.x * Time.deltaTime * mouseSens, 0f);
        camAngle += mouseInput.y * mouseSens * Time.deltaTime;
        camAngle = Mathf.Clamp(camAngle, -90, 90);
        playerCam.transform.rotation = transform.rotation * Quaternion.Euler(-camAngle, 0f, 0f);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hello there");
        if (collision.collider.tag == "ground") {
            Debug.Log("this ran too");
            isGrounded = true;
        }
    }

    public void playerJump(InputAction.CallbackContext ctx) {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            Debug.Log("the player jumped");
            isGrounded = false;
        }
    }

    private void OnDisable()
    {
        movement.Disable();
        looking.Disable();
        jump.Disable();
    }
}
