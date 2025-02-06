using UnityEngine;

public class playerMovementControls : MonoBehaviour
{

    public CharacterController controller;

    public float walkingSpeed = 12f;
    public float sprintSpeed = 24f;
    public float fallSpeed = -9.81f;
    public float jumpHight = 1f;

    public Transform GroundCheck;
    public LayerMask groundMask;

    public bool isGrounded;

    private float groundDistance = 0.2f; //How far the player can be from the ground
    
    Vector3 fallValocity;


    void Start()
    {

        
    }

    void Update()
    {

        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance,groundMask);

        //Reset fall speed
        if(isGrounded && fallValocity.y < 0 )
        {
            fallValocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * walkingSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            fallValocity.y = Mathf.Sqrt(jumpHight*-2 * fallSpeed);
        }


        fallValocity.y += fallSpeed * Time.deltaTime;

        if(fallValocity.y > 100)
        {
            fallValocity.y = 100;
        }

        controller.Move(fallValocity*Time.deltaTime);

    }
}
