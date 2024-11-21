using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour {
    public static DialogueUI Instance;

    // Dialogue GameObjects
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;

    // Dialogue Variables
    private string[] dialogue;
    private bool dialogueActive = false;
    private bool textDone = false;
    private float textSpeed = .05f;

    private IHasDialogue dialogueParent;

    // Line counter
    private int lineIndex;
    private int maxIndex;


    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
        dialogueBox.SetActive(false);
    }

    private void Start() {
        GameInput.Instance.OnUIContinuePressed += Instance_OnUIContinuePressed;
    }

    private void Update() {
        if (dialogueActive) { // Switch flag on to continue to next line of dialogue
            dialogueActive = false;

            EraseText();

            StartCoroutine(DisplayLine(dialogue[lineIndex]));
            textDone = false;
            lineIndex++;
        }
    }


    public void InitializeDialogue(string[] dialogueText, IHasDialogue dialogueInterface) {

        // Pause Player Input
        GameInput.Instance.PausePlayer();
        // Need to Add Pause Monster Movement

        // Initialize this scripts variables
        dialogueBox.SetActive(true);
        dialogueActive = true;
        dialogue = dialogueText;
        dialogueParent = dialogueInterface;

        // If line index is equal to max index, next continue will exit the dialogue
        lineIndex = 0;
        maxIndex = dialogueText.Length;
    }

    private void Instance_OnUIContinuePressed(object sender, System.EventArgs e) {
        if (!textDone) {
            textSpeed = 0f;
        }
        else {
            if (lineIndex >= maxIndex) {
                dialogueBox.SetActive(false);
                dialogueParent.DialogueDone = true;
                GameInput.Instance.UnPausePlayer();
            }
            else {
                dialogueActive = true; // Continue with next line
            }
        }
    }

    

    private IEnumerator DisplayLine(string line) {
        foreach (char c in line.ToCharArray()) {
            // Add TMP logic here 
            dialogueText.text = dialogueText.text + c;
            yield return new WaitForSeconds(textSpeed);
        }
        textDone = true;
        textSpeed = .05f;
    }

    private void EraseText() {
        dialogueText.text = "";
    }
}
