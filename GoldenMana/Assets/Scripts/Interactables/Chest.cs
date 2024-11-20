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
    [SerializeField] private string[] dialog;

    public string[] Dialogue { get; set; }
    public bool DialogueDone { get; set; }


    private bool isOpened = false;


    private void Awake() {
        Dialogue = dialog;
    }

    private void Update() {
        if (DialogueDone) {
            // Invoke chest opened event??????
            OnChestOpened?.Invoke(this, new OnChestOpenedEventArgs { ItemID = outputItem });
        }
    }


    protected override void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        if (!isOpened && hasPlayer) {
            isOpened = true;
            sr.sprite = openedChestSprite;

            OpenDialogueUI();
        }
    }


    public void OpenDialogueUI() {
        // Set timescale to 0

        // Get UI Instance open dialogue box
        StartCoroutine(DialogueUI.Instance.OpenUIDialogue(Dialogue, this));

        // Pass string dialogue to UI Instance

        // On dialogue box done.. Invoke OnChestOpened Event
    }
}
