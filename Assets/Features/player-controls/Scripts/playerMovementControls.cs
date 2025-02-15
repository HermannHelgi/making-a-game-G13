using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovementControls : MonoBehaviour
{

    public CharacterController controller;
    public float walkingSpeed = 0f;
    public float sprintSpeed = 0f;
    public float fallSpeed = -9.81f;
    public float jumpHight = 2f;

    public KeyCode sprintKey = KeyCode.LeftShift;
    public Transform GroundCheck;
    public LayerMask groundMask;
    public bool isGrounded;
    public bool isSprinting;

    private float groundDistance = 0.2f; //How far the player can be from the ground
    
    Vector3 fallValocity;
    float currentSpeed;


    void Start()
    {
        currentSpeed = walkingSpeed;
    }

    void applyGravity()
    {

        fallValocity.y += fallSpeed * Time.deltaTime;
        //Limit the max fall speed;
        if(fallValocity.y > 100)
        {
            fallValocity.y = 100;
        }

        controller.Move(fallValocity*Time.deltaTime);
    }

    void applyMovement()
    {
        
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if(isGrounded && Input.GetKey(sprintKey))
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
            if(currentSpeed > walkingSpeed)
            {
                currentSpeed -= 6.0f * Time.deltaTime;
            }
        }
        if(isSprinting && isGrounded)
        {
            if(currentSpeed < sprintSpeed)
            {
                currentSpeed += 4.5f * Time.deltaTime;
            }
        }


        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void performeJump()
    {
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            fallValocity.y = Mathf.Sqrt(jumpHight*-2 * fallSpeed);
        }
    }

    void Update()
    {
        

        {
            //Ground check
            isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance,groundMask);
            //Reset fall speed
            if(isGrounded && fallValocity.y < 0 )
            {
                fallValocity.y = -2f;
            }
        }

        applyMovement();

        performeJump();

        applyGravity();



    }
}
