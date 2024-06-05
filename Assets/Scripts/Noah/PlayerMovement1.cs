using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using System;
using JetBrains.Annotations;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

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
    public InputAction pickUp;
    [SerializeField]
    public float playerSpeed = 20f, mouseSens = 0f;
    [SerializeField]
    public Camera playerCam;
    [SerializeField]
    public GameObject inventory;
    [SerializeField]
    public GameObject radialInventory;
    [SerializeField]
    public GameObject pickUpItemText;

    bool isGrounded = true;
    Rigidbody rb;
    Vector2 strafeInput;
    Vector2 mouseInput;
    Vector3 playerVelocity;
    float camAngle;
    InventorySystem myInventory;
    List<GameObject> lastItemTouched;

    void Start()
    {
        movement.Enable();
        looking.Enable();
        jump.Enable();
        drop.Enable();
        pickUp.Enable();
        jump.performed += playerJump;
        drop.performed += playerDropItem;
        rb = GetComponent<Rigidbody>();
        //cursor.lockstate = cursorlockmode.locked;
        //cursor.visible = false;
        myInventory = inventory.GetComponent<InventorySystem>();
        lastItemTouched = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = looking.ReadValue<Vector2>();
        strafeInput = movement.ReadValue<Vector2>();
        playerVelocity = new Vector3(strafeInput.x, 0f, strafeInput.y);
        playerVelocity = transform.TransformDirection(playerVelocity);
        moveCamera();
        pickUpLastItem();
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

        //Debug.Log(collision.collider.gameObject.name);

        if (collision.collider.tag == "ground") {
            isGrounded = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            if (!lastItemTouched.Contains(other.gameObject))
            {
                if (myInventory.canAddItem(other.gameObject.GetComponent<ItemObject>().referenceItem))
                {
                    pickUpItemText.GetComponent<TextMeshProUGUI>().text = "Press E to pickup";
                }
                else
                {
                    pickUpItemText.GetComponent<TextMeshProUGUI>().text = "Inventory full :(";
                }
                lastItemTouched.Add(other.gameObject);
            }
        }
    }

    public void pickUpLastItem()
    {
        if (lastItemTouched.Count == 0 || pickUp.ReadValue<float>() != 1f) return;

        if (lastItemTouched[0].GetComponent<ItemObject>().canPickUp)
        {
            myInventory.PickUpItem(lastItemTouched[0].GetComponent<ItemObject>().referenceItem, lastItemTouched[0]);
            lastItemTouched.Remove(lastItemTouched[0]);
            pickUpItemText.GetComponent<TextMeshProUGUI>().text = "";
        }
    }


    public void OnTriggerExit(Collider other)
    {
        pickUpItemText.GetComponent<TextMeshProUGUI>().text = "";
        if (other.gameObject.tag == "item") { 
            lastItemTouched.Remove(other.gameObject);
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
        myInventory.DropItem(transform, playerCam.transform);
    }

    private void OnDisable()
    {
        movement.Disable();
        looking.Disable();
        jump.Disable();
        drop.Disable();
        drop.Disable();
    }
}
