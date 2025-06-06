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
    public event EventHandler<OnPlayerHitEventArgs> OnPlayerHit;
    public class OnPlayerHitEventArgs : EventArgs {
        public int Damage;
    }

    private LayerMask floorLayer;

    private float moveDir;
    private float moveDirStatic;
    private Rigidbody2D rb;

    public bool onFloor { get; private set; } = false;
    private bool onSlopeLeft = false;
    private bool onSlopeRight = false;
    private bool isJumping = false;
    private float startJumpHeight;

    private bool vulnerable = true;
    private float vulnerabilityTimer;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;

        floorLayer = LayerMask.GetMask("Floor", "Breakable");
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


        // Resets vulnerability
        if (!vulnerable) {
            vulnerabilityTimer -= Time.deltaTime;
            if (vulnerabilityTimer <= 0) {
                vulnerable = true;
            }
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

        onSlopeLeft = Physics2D.Raycast(raySlopeOriginLeft, Vector2.down, .42f, floorLayer);
        onSlopeRight = Physics2D.Raycast(raySlopeOriginRight, Vector2.down, .42f, floorLayer);

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
            float maxMoveSpeed = 4.8f;

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

        
    }

    private void FixedUpdate() {
        // Handle slopes
        if (onFloor) {
            float forceY = 59f;
            if (onSlopeLeft || onSlopeRight) { rb.AddForceY(forceY, ForceMode2D.Force); }
        }
    }

    public float GetMoveDirectionStaticX() => moveDirStatic;  // Get facing direction of player



    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Monster")) {
            // Sets invulnerability
            if (vulnerable) {
                vulnerable = false;
                vulnerabilityTimer = .3f;
                // Adds backwards force to player
                collision.gameObject.TryGetComponent<BaseMonster>(out var monsterScript);
                float backMultiplier = 2;
                float backForce = (transform.position - collision.transform.position).normalized.x * backMultiplier;
                float upwardForce = 2;
                rb.AddForce(new Vector2(backForce, upwardForce), ForceMode2D.Impulse);
                // Fire event to player script
                OnPlayerHit?.Invoke(this, new OnPlayerHitEventArgs { Damage = monsterScript.Damage });
            }
            collision.gameObject.TryGetComponent<Minotaur>(out var minotaurScript);
            if (minotaurScript) {
                if (minotaurScript.GetIsFalling()) {
                    // If minotaur is falling
                    rb.AddForceX(minotaurScript.GetFallDirection() * 5, ForceMode2D.Impulse);
                }
            }

            
        }
    }


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
