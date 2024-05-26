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
    public InputAction drop;
    [SerializeField]
    public float playerSpeed = 20f, mouseSens = 0f;
    [SerializeField]
    public Camera playerCam;
    [SerializeField]
    public GameObject inventory;

    private dropItem dropItemReference;
    bool isGrounded = true;
    Rigidbody rb;
    Vector2 strafeInput;
    Vector2 mouseInput;
    Vector3 playerVelocity;
    float camAngle;
    InventorySystem myInventory;

    void Start()
    {
        movement.Enable();
        looking.Enable();
        jump.Enable();
        drop.Enable();
        jump.performed += playerJump;
        drop.performed += playerDropItem;
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        myInventory = inventory.GetComponent<InventorySystem>();
        dropItemReference = GetComponent<dropItem>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = looking.ReadValue<Vector2>();
        strafeInput = movement.ReadValue<Vector2>();
        playerVelocity = new Vector3(strafeInput.x, 0f, strafeInput.y);
        playerVelocity = transform.TransformDirection(playerVelocity);
        moveCamera();
    }

    void moveCamera()
    {
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
        if (collision.collider.tag == "ground") {
            isGrounded = true;
        }

        if (collision.collider.tag == "item")
        {
            collision.collider.GetComponent<ItemObject>().pickUpItem();
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

    public void playerDropItem(InputAction.CallbackContext ctx)
    {
        myInventory.DropItem();
    }

    private void OnDisable()
    {
        movement.Disable();
        looking.Disable();
        jump.Disable();
        drop.Disable();
    }
}
