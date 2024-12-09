using UnityEngine;
using System;

public class HealthPickup : BaseInteractable, IHasDialogue {

    [SerializeField] private string[] dialogue;

    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }


    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;
        // Interface implementation
        Dialogue = dialogue;
        HasChoice = false;


        // Delete health pick up if it has already been picked up
        foreach (Vector2 healthPickupPos in SaveManager.Instance.selectedScene.healthPickupPos) {
            if (Vector2.Distance(healthPickupPos, transform.position) < 1) {
                Destroy(gameObject);
            }
        }

    }


    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        if (hasPlayer) {
            ((IHasDialogue)this).StartDialogue();
        }

    }


    public void DialogueDone() {
        // Add 4 to player maxHealth
        Player.Instance.maxHealth += 4;
        Player.Instance.health += 4;

        // Save item to SaveManager
        SaveManager.Instance.selectedScene.healthPickupPos.Add(transform.position);

        Destroy(gameObject);
    }
}
