using System;
using UnityEngine;

public class Chest : BaseInteractable , IHasDialogue
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite openedChestSprite;
    [SerializeField] private string outputItem;
    [SerializeField] private string[] dialogue;

    public string[] Dialogue { get; set; }
    public bool DialogueDone { get; set; }
    public bool HasChoice { get; set; }

    private bool isOpened = false;


    private void Awake() {
        Dialogue = dialogue; // Assign dialogue to the interface
    }

    private void Update() {
        if (DialogueDone) {
            DialogueDone = false;
            Player.Instance.RecieveItem(outputItem);
        }
    }


    protected override void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        if (!isOpened && hasPlayer) {
            isOpened = true;
            sr.sprite = openedChestSprite;

            // Call Dialog UI
            ((IHasDialogue)this).StartDialogue();
        }
    }
}
