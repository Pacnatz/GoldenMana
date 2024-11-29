using UnityEngine;

public class Player : MonoBehaviour , IHasDialogue
{
    public static Player Instance { get; private set; }

    // Attack Unlock Variables
    public bool fireballUnlocked { get; private set; }
    // Dialogue Interface Variables
    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }

    // Serializables
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

        // Interface initialization
        Dialogue = new string[] { "You have died..", "Try again?" };
        HasChoice = true; // Death choice
    }

    private void Update() {

        // Player death
        if (health <= 0) {
            GameObject deathParticles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(deathParticles, 1f);
            ((IHasDialogue)this).StartDialogue();
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
                UnlockFireSpell();
                break;
            default:
                Debug.LogError($"{itemID} is not a valid item");
                break;
        }
    }

    public void UnlockFireSpell() {
        playerAttack.UnlockFireSpell();
        fireballUnlocked = true;
    }

    void IHasDialogue.DialogueDone() {
        // Add actions here if needed
    }


}
