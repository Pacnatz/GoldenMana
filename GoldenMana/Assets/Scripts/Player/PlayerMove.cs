using System;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    public event EventHandler OnInteractPressed;
    public event EventHandler<OnJumpActionEventArgs> OnJumpAction;
    public class OnJumpActionEventArgs : EventArgs {
        public bool IsJumping;
    }

    [SerializeField] private LayerMask floorLayer;

    private float moveDir;
    private float moveDirStatic;
    private Rigidbody2D rb;

    public bool onFloor { get; private set; } = false;
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

        if (moveDir != 0) { // Sets which way player is facing
            moveDirStatic = moveDir;
        }

    }

    private void HandleMovement() {

        // Test floor
        float playerWidth = .25f;
        float playerOffset = playerWidth / 1.5f;
        float playerSlopeOffset = playerWidth / 1.25f;
        RaycastHit2D isGrounded = Physics2D.CircleCast(transform.position, playerWidth, Vector2.down, .45f, floorLayer);  // Modify distance if character sprite changes

        // Testing both sides for ground
        Vector2 rayOriginLeft = new Vector2(transform.position.x - playerOffset, transform.position.y);
        Vector2 rayOriginRight = new Vector2(transform.position.x + playerOffset, transform.position.y);

        bool isGroundedLeft = Physics2D.Raycast(rayOriginLeft, Vector2.down, .5f, floorLayer);
        bool isGroundedRight = Physics2D.Raycast(rayOriginRight, Vector2.down, .5f, floorLayer);

        // Testing both sides for slope
        Vector2 raySlopeOriginLeft = new Vector2(transform.position.x - playerSlopeOffset, transform.position.y);
        Vector2 raySlopeOriginRight = new Vector2(transform.position.x + playerSlopeOffset, transform.position.y);

        bool onSlopeLeft = Physics2D.Raycast(raySlopeOriginLeft, Vector2.down, .42f, floorLayer);
        bool onSlopeRight = Physics2D.Raycast(raySlopeOriginRight, Vector2.down, .42f, floorLayer);

        // Ground check
        if (isGrounded || onSlopeLeft || onSlopeRight) { onFloor = true; }
        else { onFloor = false; }

        // Test ceiling
        RaycastHit2D hitCeiling = Physics2D.CircleCast(transform.position, playerWidth / 2, Vector2.up, .375f, floorLayer);  // Modify distance if character sprite changes
        if (hitCeiling) {
            isJumping = false;
        }

        // Handle Jump
        float jumpDeceleration;
        float minJumpHeight = 1f;
        if (isJumping) { // Turned on from OnJumpPressed Event

            if (transform.position.y < startJumpHeight + minJumpHeight) {
                float jumpSpeed = 6f;
                rb.linearVelocityY = jumpSpeed;
                
            }
            else {
                jumpDeceleration = 8.5f;
                rb.linearVelocityY -= jumpDeceleration * Time.deltaTime;
            }
            if (rb.linearVelocityY <= 0) { // If decelerated to 0
                isJumping = false;
            }

        }
        else {
            if (!isGroundedLeft && !isGroundedRight) { // If theres not ground on both sides
                if (rb.linearVelocityY <= 0) { // Fire event if on floor OR yVelocity going down
                    OnJumpAction?.Invoke(this, new OnJumpActionEventArgs { IsJumping = false });
                }
                float minVelocityY = -11f;
                float fallSpeed = 2f;
                rb.linearVelocityY = Mathf.Lerp(rb.linearVelocityY, minVelocityY, fallSpeed * Time.deltaTime);
            }
            else { // Handle falling down corners with capsulecollider2D
                rb.linearVelocityY = 0;
                OnJumpAction?.Invoke(this, new OnJumpActionEventArgs { IsJumping = false });
            }
            
        }

        // Handle X Movement *slidy x movement*
        if (moveDir != 0) {  // Acceleration
            float accelerationRate = 7f;
            float opposingDecelerationRate = 22f; // Deceleration rate when input is in opposite direction
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
            float decelerationRate = 15f;  // Decleration rate when no input
            if (rb.linearVelocityX > 0.05f) {
                rb.linearVelocityX -= decelerationRate * Time.deltaTime;
            }
            else if (rb.linearVelocityX < -0.05f) {
                rb.linearVelocityX += decelerationRate * Time.deltaTime;
            }
            else {
                rb.linearVelocityX = 0;
            }
        }

        // Handle slopes
        if (onFloor) {
            float forceY = 8f;
            if (onSlopeLeft || onSlopeRight) { rb.AddForceY(forceY, ForceMode2D.Force); }
        }
    }

    public float GetMoveDirectionStaticX() => moveDirStatic;  // Get facing direction of player


    private void Input_OnJumpPressed(object sender, EventArgs e) {
        if (onFloor) {
            if (rb != null) { // Some reason need this test or bad things will happen
                isJumping = true;
                startJumpHeight = transform.position.y;
            }
            OnJumpAction?.Invoke(this, new OnJumpActionEventArgs { IsJumping = true });
        }
    }
    private void Input_OnJumpCanceled(object sender, EventArgs e) {
        isJumping = false;
    }

    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        OnInteractPressed?.Invoke(this, EventArgs.Empty);
    }

    // Input Action Reference
}
