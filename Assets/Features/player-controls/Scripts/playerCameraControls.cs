// using Unity.Mathematics;
using UnityEngine;

public class playerCameraControl : MonoBehaviour
{

    public float sensitivity = 100f;
    public Transform playerMesh;

    float rotationX = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX,-90f,90f);


        transform.localRotation = Quaternion.Euler(rotationX,0f,0f);
        playerMesh.Rotate(Vector3.up * mouseX);
    }
}