using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    CharacterController controller;
    [SerializeField]
    Transform groundCheck;

    [SerializeField]
    float speed = 12f;
    [SerializeField]
    float groundDistance = 0.4f;

    [Tooltip("Literally height, it takes gravity to calculate needed force")]
    [SerializeField]
    float jumpHeight = 5f;

    [SerializeField]
    LayerMask groundMask;

    [Header("Gravity")]
    [SerializeField]
    float gravity = -9.81f;
    [SerializeField]
    float maxFallSpeed = -100;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);

        controller.Move(velocity * Time.deltaTime);

        
    }
}
