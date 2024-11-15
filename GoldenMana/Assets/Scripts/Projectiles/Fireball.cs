using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private float fireballSpeed = 20f;
    private Vector2 velocity;
    private Vector2 startPos;
    private Vector2 endPos;
    private float range = 5f;
    private PlayerAttack attackScript;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, .5f);
    }

    private void Update()
    {
        rb.linearVelocity = velocity * fireballSpeed;
    }

    public void InitializeFireball(PlayerAttack attackScript, Vector2 velocity) {
        this.attackScript = attackScript;
        this.velocity = velocity;
        startPos = transform.position;
        if (velocity.x > 0) {
            
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor")) {
            Destroy(gameObject);
        }
    }
    private void OnDestroy() {
        attackScript.DeductFireBallsActive();
    }


}
