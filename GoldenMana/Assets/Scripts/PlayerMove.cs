using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] private LayerMask floorLayer;

    private float moveDir;
    private float moveSpeed = 10f;
    private Rigidbody2D rb;

    private bool onFloor = false;
    private bool isJumping = false;







    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        GameInput.Instance.OnJumpPressed += Input_OnJumpPressed;
        GameInput.Instance.OnJumpCanceled += Input_OnJumpCanceled;
    }

    
    private void Update() {

        Debug.Log(onFloor);

        HandleMovement();
        moveDir = GameInput.Instance.GetMovementVectorX();

        rb.linearVelocityX = moveDir * moveSpeed;
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



}
