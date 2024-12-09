using UnityEngine;

public class ManaPickup : BaseInteractable {

    private Rigidbody2D rb;
    private LayerMask floorLayer;
    private int manaAmount = 1;



    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        floorLayer = LayerMask.GetMask("Floor");

        float xVel = Random.Range(-5f, 5f);
        float yVel = Random.Range(0f, 5f);
        rb.AddForce(new Vector2(xVel, yVel), ForceMode2D.Impulse);

        float zTorque = Random.Range(-30f, 30f);
        rb.AddTorque(zTorque);
    }

    private void Update() {
        if (hasPlayer) {
            Player.Instance.mana += manaAmount;
            PlayerUI.Instance.HandleFlash();
            Destroy(gameObject);
        }

        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, .21f, floorLayer);

        if (hitGround) {
            float slowDownRate = 10000f;
            rb.linearVelocityY -= rb.linearVelocityY / (slowDownRate * Time.deltaTime);
            rb.linearVelocityX -= rb.linearVelocityX / (slowDownRate * Time.deltaTime);
        }
    }

}
