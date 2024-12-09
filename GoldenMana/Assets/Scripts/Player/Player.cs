using UnityEngine;

public class Player : MonoBehaviour , IHasDialogue
{
    public static Player Instance { get; private set; }

    // Attack Unlock Variables
    
    // Dialogue Interface Variables
    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }

    // Serializables
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private GameObject deathParticlesPrefab;

    [HideInInspector] public bool fireballUnlocked;
    [HideInInspector] public bool minotaurKilled;
    [HideInInspector] public int health;
    [HideInInspector] public int maxHealth;

    [HideInInspector] public int mana;
    [HideInInspector] public int manaLevel;
    private int maxMana1 = 5;
    private int maxMana2 = 10;
    private int maxMana3 = 15;



    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;


        // Interface initialization
        Dialogue = new string[] { "You have died..", "Try again?" };
        HasChoice = true; // Death choice

        maxHealth = 12;
        health = maxHealth;

        mana = 0;
        manaLevel = 1;

        // Debugging
        UnlockFireSpell();

        
    }

    private void Start() {
        PlayerMove.Instance.OnPlayerHit += Instance_OnPlayerHit;
    }



    private void Update() {

        health = Mathf.Clamp(health, 0, maxHealth);

        // Mana Logic Increasing
        switch (manaLevel) {
            case 1:
                if (mana >= maxMana1) {
                    manaLevel++;
                    mana = 0;
                }
                break;
            case 2:
                if (mana >= maxMana2) {
                    manaLevel++;
                    mana = 0;
                }
                break;
            case 3:
                mana = Mathf.Clamp(mana, -1000, maxMana3);
                break;
        }

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

    // On take damage
    private void Instance_OnPlayerHit(object sender, PlayerMove.OnPlayerHitEventArgs e) {
        health -= e.Damage;

        int manaDamageMultiplier = 2;
        int startingMana = mana;
        mana -= e.Damage * manaDamageMultiplier;

        if (mana < 0 && manaLevel > 1) {
            manaLevel--;
            switch (manaLevel) {
                case 1:
                    mana = maxMana1 - (Mathf.Abs(e.Damage * manaDamageMultiplier - startingMana));
                    break;
                case 2:
                    mana = maxMana1 - (Mathf.Abs(e.Damage * manaDamageMultiplier - startingMana));
                    break;
            }
        }

        mana = Mathf.Clamp(mana, 0, maxMana3);
    }

    void IHasDialogue.DialogueDone() {
        // Add actions here if needed
    }




}
