using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private ShipMotor motor;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 1.5f;

    private void Awake()
    {
        if (motor == null)
            motor = GetComponent<ShipMotor>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float thrust = Input.GetAxisRaw("Vertical");
        float strafe = Input.GetAxisRaw("Horizontal");

        float lift = 0f;
        if (Input.GetKey(KeyCode.Space)) lift += 1f;
        if (Input.GetKey(KeyCode.LeftControl)) lift -= 1f;

        float yaw = Input.GetAxis("Mouse X") * mouseSensitivity;
        float pitch = -Input.GetAxis("Mouse Y") * mouseSensitivity;

        float roll = 0f;
        if (Input.GetKey(KeyCode.Q)) roll += 1f;
        if (Input.GetKey(KeyCode.E)) roll -= 1f;

        motor.SetInput(thrust, strafe, lift, pitch, yaw, roll);
    }
}
