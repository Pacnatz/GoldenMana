using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private PlayerAttack playerAttack;

    private int health;
    private int maxHealth;



    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
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
