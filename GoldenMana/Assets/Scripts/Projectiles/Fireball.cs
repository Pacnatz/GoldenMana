using UnityEngine;
using UnityEngine.Tilemaps;
public class Fireball : MonoBehaviour
{
    [SerializeField] private LayerMask breakableLayer;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject smokeParticlePrefab;

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


    public void InitializeFireball(PlayerAttack attackScript, Vector2 direction) {
        this.attackScript = attackScript;
        this.direction = direction;

        startPos = transform.position;

        endPos = startPos + direction * range;

    }

    private void Update()
    {
        rb.linearVelocity = direction * fireballSpeed;

        if (Vector2.Distance(transform.position, endPos) < 1) {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }


        // Hit breakable layer
        float projectileWidth = .5f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, projectileWidth, breakableLayer);

        if (hit) {
            Tilemap breakableTileMap = hit.collider.transform.GetComponent<Tilemap>();
            var worldHitPos = hit.point + direction * projectileWidth;
            var cellHitPos = breakableTileMap.WorldToCell(worldHitPos);

            // Saving broken blocks
            SaveManager.Instance.selectedScene.BrokenCellPos.Add(cellHitPos);

            breakableTileMap.SetTile(cellHitPos, null);
            GameObject smokeParticles = Instantiate(smokeParticlePrefab, worldHitPos, Quaternion.identity);
            Destroy(smokeParticles, 1f);
            Destroy(gameObject);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor") || collision.gameObject.layer == LayerMask.NameToLayer("Spikes")) {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    private void OnDestroy() {
        
        attackScript.DeductFireBallsActive();
    }


}
