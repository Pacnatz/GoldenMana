using UnityEngine;

public interface IHasDialogue {

    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }
    public bool DialogueDone { get; set; }

    public void StartDialogue() {
        DialogueUI.Instance.InitializeDialogue(Dialogue, this, HasChoice);
    }


}