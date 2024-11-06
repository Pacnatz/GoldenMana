using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    public event EventHandler OnInteractPressed;

    [SerializeField] private LayerMask floorLayer;

    private float moveDir;
    private Rigidbody2D rb;

    private bool onFloor = false;
    private bool isJumping = false;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        GameInput.Instance.OnJumpPressed += Input_OnJumpPressed;
        GameInput.Instance.OnJumpCanceled += Input_OnJumpCanceled;
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;
    }

    

    private void Update() {

        moveDir = GameInput.Instance.GetMovementVectorX();
        HandleMovement();
        

    }

    private void HandleMovement() {

        // Test floor
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, .7f, floorLayer);
        if (hit) { onFloor = true; }
        else { onFloor = false; }

        // Handle Jump
        if (isJumping) {
            float jumpDeceleration = 5f;
            rb.linearVelocityY -= jumpDeceleration * Time.deltaTime;

            if (rb.linearVelocityY <= 0) { //If decelerated to 0
                isJumping = false;
            }
        }
        else {
            float minVelocityY = -10f;
            float fallSpeed = 1.5f;
            rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, minVelocityY, fallSpeed * Time.deltaTime);
        }

        // Handle X Movement

        if (moveDir != 0) {  // Acceleration
            float accelerationRate = 10f;
            float decelerationRate = 30f;
            float maxMoveSpeed = 4f;
            if (rb.linearVelocityX > 0 && moveDir < 0) { // Handle opposite XMovement
                rb.linearVelocityX += moveDir * decelerationRate * Time.deltaTime;
            }
            else if (rb.linearVelocityX < 0 && moveDir > 0) { // Handle opposite XMovement
                rb.linearVelocityX += moveDir * decelerationRate * Time.deltaTime;
            }
            else {
                rb.linearVelocityX += moveDir * accelerationRate * Time.deltaTime;
            }
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxMoveSpeed, maxMoveSpeed);
        }
        else {  // Deceleration
            float decelerationRate = 20f;
            if (rb.linearVelocityX > 0) {
                rb.linearVelocityX -= decelerationRate * Time.deltaTime;
            }
            else {
                rb.linearVelocityX += decelerationRate * Time.deltaTime;
            }
        }

    }


    private void Input_OnJumpPressed(object sender, System.EventArgs e) {
        if (onFloor) {
            isJumping = true;
            float jumpSpeed = 6f;
            rb.linearVelocityY = jumpSpeed;
        }
    }
    private void Input_OnJumpCanceled(object sender, System.EventArgs e) {
        isJumping = false;
    }

    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        OnInteractPressed?.Invoke(this, EventArgs.Empty);
    }

    // Input Action Reference
}
