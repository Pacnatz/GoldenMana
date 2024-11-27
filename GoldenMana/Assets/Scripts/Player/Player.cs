using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private GameObject deathParticlesPrefab;

    private int health;
    private int maxHealth = 100;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;

        health = maxHealth;
    }

    private void Update() {

        // Player death
        if (health <= 0) {
            GameObject deathParticles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(deathParticles, 1f);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Spikes")) {
            health = 0;
        }
    }
    // Called once by chest
    public void RecieveItem(string itemID) {
        switch (itemID) {
            case "FireballSpell":
                playerAttack.UnlockFireSpell();
                break;
            default:
                Debug.LogError($"{itemID} is not a valid item");
                break;
        }
    }
}
