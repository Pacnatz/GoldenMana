using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    private float moveDir;
    private float moveSpeed = 10;
    private Rigidbody2D rb;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        moveDir = GameInput.Instance.GetMovementVectorX();

        rb.linearVelocityX = moveDir* moveSpeed;
    }
}
