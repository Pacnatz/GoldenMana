using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerAttack playerAttack;
    private int health;
    private int maxHealth;

    private float mana;
    private float maxMana;
    private float manaFillRate; // Fill rate per second
    private float manaCoolDownTimer;
    private bool canRegenMana = true;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start() {
        playerAttack.OnManaUsed += PlayerAttack_OnManaUsed;

        maxMana = 3f;
        manaFillRate = 1f;
    }

    private void Update() {
        
        if (!canRegenMana) {
            manaCoolDownTimer -= Time.deltaTime;
            if (manaCoolDownTimer <= 0) {
                canRegenMana = true;
            }
        }

        if (mana < maxMana && canRegenMana) {
            mana += manaFillRate * Time.deltaTime;
            
        }
        mana = Mathf.Clamp(mana, 0, maxMana);

    }

    public float GetMana() => mana;

    private void PlayerAttack_OnManaUsed(object sender, PlayerAttack.OnManaUsedEventArgs e) {
        mana -= e.mana;
        manaCoolDownTimer = e.timeToContinueRegenerateMana;
        canRegenMana = false;

    }
}
