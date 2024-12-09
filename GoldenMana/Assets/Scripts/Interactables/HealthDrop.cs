using UnityEngine;

public class HealthDrop : BaseInteractable {

    [SerializeField] private int healAmount = 1;


    private void Update() {
        if (hasPlayer) {
            Player.Instance.health += healAmount;
            Destroy(gameObject);
        }
    }
}
