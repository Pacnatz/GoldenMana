using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rb;
    private float fireballSpeed = 20f;
    private Vector2 direction;
    private Vector2 startPos;
    private Vector2 endPos;
    private float range = 5f;
    private PlayerAttack attackScript;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.linearVelocity = direction * fireballSpeed;

        if (Vector2.Distance(transform.position, endPos) < 1) {
            Destroy(gameObject);
        }
    }

    public void InitializeFireball(PlayerAttack attackScript, Vector2 direction) {
        this.attackScript = attackScript;
        this.direction = direction;

        startPos = transform.position;

        endPos = startPos + direction * range;

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
