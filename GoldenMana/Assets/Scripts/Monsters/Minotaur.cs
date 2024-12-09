using UnityEngine;

public class Minotaur : BaseMonster, IHasDialogue {


    private bool isActive = false;

    // Dialogue variables
    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }

    private void Awake() {
        // Initialization
        isBossMonster = true;
        Dialogue = new string[] { "Prepare to die!" };
        ((IHasDialogue)this).StartDialogue();
    }

    protected override void Update() {
        if (isActive) {
            base.Update();
        }
    }
    public void DialogueDone() {
        isActive = true;
    }
}
