using UnityEngine;
using System;

public class SavePoint : BaseInteractable , IHasDialogue{
    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }

    [SerializeField] private string sceneName;

    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;

        Dialogue = new string[] { "You have saved." };
    }

    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        if (hasPlayer) {
            ((IHasDialogue)this).StartDialogue();
        }
    }

    public void DialogueDone() {
        SaveManager.Instance.Save(transform.position);
    }
}
