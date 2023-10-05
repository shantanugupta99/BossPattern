using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform groundcheck;
    public Transform cam;

    public LayerMask groundMask;

    Vector3 velocity;

    public int maxHP = 3;  // Setting a default value of 100 for max HP.
    private int currentHP;  // Player's current HP.

    public float speed = 6f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float turnSmoothTime = 0.1f;

    public float boostDuration = 2f;
    public float boostCooldown = 3f;
    public float boostedSpeedMultiplier = 2f;  // Assuming you want to double the speed during the boost.
    private float boostEndTime = 0f;
    private float boostCooldownEndTime = 0f;

    float turnSmoothVelocity;

    private bool isBoosting = false;
    bool isGrounded;

    private void Start()
    {
        currentHP = maxHP;  // Initialize the player's health to the max value.
    }


    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundcheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > boostCooldownEndTime)
        {
            StartBoost();
            Debug.Log("Boosted");
        }

        // If boosting, and boost time has ended, reset the speed.
        if (isBoosting && Time.time > boostEndTime)
        {
            EndBoost();
            Debug.Log("Not Boosted");
        }

        float currentSpeed = isBoosting ? speed * boostedSpeedMultiplier : speed;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);

        velocity.y += gravity * Time.deltaTime;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, 
                          targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
            controller.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
            controller.Move(velocity * Time.deltaTime);
        }
    }

    void StartBoost()
    {
        isBoosting = true;
        boostEndTime = Time.time + boostDuration;
        boostCooldownEndTime = Time.time + boostDuration + boostCooldown;
    }

    void EndBoost()
    {
        isBoosting = false;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;  // Deduct the damage from the player's current HP.
        Debug.Log($"Player HP: {currentHP}");  // Display the player's HP in the console. 

        if (currentHP <= 0)  // Check if the player's health is zero or below.
        {
            Die();  // If so, invoke a method to handle the player's death.
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");  // Just a log for now
    }
}
