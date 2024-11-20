using UnityEngine;

public interface IHasDialogue {

    public string[] Dialogue { get; set; }
    public bool DialogueDone { get; set; }

    public void OpenDialogueUI();

}
