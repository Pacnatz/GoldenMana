using System;
using UnityEngine;

public class Chest : BaseInteractable , IHasDialogue
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite openedChestSprite;
    [SerializeField] private string outputItem;
    [SerializeField] private string[] dialogue;

    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }

    private bool isOpened = false;


    private void Awake() {
        Dialogue = dialogue; // Assign dialogue to the interface
    }

    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed1;
    }

    private void Instance_OnInteractPressed1(object sender, EventArgs e) {
        if (!isOpened && hasPlayer) {
            isOpened = true;
            sr.sprite = openedChestSprite;
            // Call Dialog UI
            ((IHasDialogue)this).StartDialogue();
        }
    }


    public void DialogueDone() {
        Player.Instance.RecieveItem(outputItem);
    }

}
