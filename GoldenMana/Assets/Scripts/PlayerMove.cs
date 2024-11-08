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
    private float startJumpHeight;
    

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
        float playerWidth = .25f;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, playerWidth, Vector2.down, .7f, floorLayer);  // Distance is player height / 2 + 0.2
        if (hit) { onFloor = true; }
        else { onFloor = false; }

        // Handle Jump
        float jumpDeceleration;
        float minJumpHeight = 1;
        if (isJumping) {

            if (transform.position.y < startJumpHeight + minJumpHeight) {
                float jumpSpeed = 6f;
                rb.linearVelocityY = jumpSpeed;
            }
            else{
                jumpDeceleration = 8.9f;
                rb.linearVelocityY -= jumpDeceleration * Time.deltaTime;
            }
            if (rb.linearVelocityY <= 0) { //If decelerated to 0
                    isJumping = false;
            }
            
        }
        else {
            float minVelocityY = -11f;
            float fallSpeed = 2f;
            rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, minVelocityY, fallSpeed * Time.deltaTime);
        }

        // Handle X Movement *slidy x movement*
        if (moveDir != 0) {  // Acceleration
            float accelerationRate = 10f;
            float opposingDecelerationRate = 30f; // Deceleration rate when input is in opposite direction
            float maxMoveSpeed = 4.3f;
            if (rb.linearVelocityX > 0 && moveDir < 0) { // Handle opposite XMovement
                rb.linearVelocityX += moveDir * opposingDecelerationRate * Time.deltaTime;
            }
            else if (rb.linearVelocityX < 0 && moveDir > 0) { // Handle opposite XMovement
                rb.linearVelocityX += moveDir * opposingDecelerationRate * Time.deltaTime;
            }
            else {  // Handle acceleration
                rb.linearVelocityX += moveDir * accelerationRate * Time.deltaTime;
            }
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -maxMoveSpeed, maxMoveSpeed);
        }
        else {  // Deceleration
            float decelerationRate = 20f;  // Decleration rate when no input
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
            if (rb != null) {
                isJumping = true;
                startJumpHeight = transform.position.y;
            }

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
