using UnityEngine;

public interface IHasDialogue {

    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }


    public void DialogueDone();

    public void StartDialogue() {
        if (DialogueUI.Instance) {
            DialogueUI.Instance.InitializeDialogue(Dialogue, this, HasChoice);
        }
        else {
            Debug.LogWarning("No DialogueUI script found");
        }
    }


}
