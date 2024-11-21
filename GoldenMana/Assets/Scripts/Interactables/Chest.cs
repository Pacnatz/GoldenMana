using System;
using UnityEngine;

public class Chest : BaseInteractable , IHasDialogue
{
    public event EventHandler<OnChestOpenedEventArgs> OnChestOpened;
    public class OnChestOpenedEventArgs : EventArgs {
        public string ItemID; // String for delegates to determine which item
    }

    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite openedChestSprite;
    [SerializeField] private string outputItem;
    [SerializeField] private string[] dialogue;

    public string[] Dialogue { get; set; }
    public bool DialogueDone { get; set; }


    private bool isOpened = false;


    private void Awake() {
        Dialogue = dialogue; // Assign dialogue to the interface
    }

    private void Update() {
        if (DialogueDone) {
            DialogueDone = false;
            // Invoke chest opened event??????
            OnChestOpened?.Invoke(this, new OnChestOpenedEventArgs { ItemID = outputItem });
            Debug.Log("WORKS!");
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
