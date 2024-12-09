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

    [HideInInspector] public bool isOpened = false;


    private void Awake() {
        Dialogue = dialogue; // Assign dialogue to the interface
    }

    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;
        foreach (Vector2 chestPos in SaveManager.Instance.selectedScene.OpenedChestPos) {
            if (Vector2.Distance(transform.position, chestPos) < 1) {
                isOpened = true;
                sr.sprite = openedChestSprite;
            }
        }
    }

    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        if (!isOpened && hasPlayer) {
            isOpened = true;
            sr.sprite = openedChestSprite;
            
            // Call Dialog UI
            ((IHasDialogue)this).StartDialogue();
        }
    }

    public void DialogueDone() {
        Player.Instance.RecieveItem(outputItem);
        SaveManager.Instance.selectedScene.OpenedChestPos.Add(transform.position);
    }

}
