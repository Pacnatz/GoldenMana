using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private float fireballSpeed = 20f;
    private Vector2 velocity;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, 3);
    }

    private void Update()
    {
        rb.linearVelocity = velocity * fireballSpeed;
    }

    public void SetInitialVelocity(Vector2 velocity) {
        this.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor")) {
            Destroy(gameObject);
        }
    }


}
