using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DialogueUI : MonoBehaviour {
    public static DialogueUI Instance;

    // Dialogue GameObjects
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject dialogueChoiceBox;
    [SerializeField] private RectTransform choiceArrow;

    // Dialogue Variables
    private string[] dialogue;
    private bool dialogueActive = false;
    private bool textDone = false;
    private float textSpeed = .05f;

    private IHasDialogue dialogueParent;

    // Line counter
    private int lineIndex;
    private int maxIndex;

    // Choice Variables
    private bool hasChoice;
    private int choice = 0;
    private bool dialogueChoiceActive = false;
    private Vector2 arrowYesPos= new Vector2(-100, 0);
    private Vector2 arrowNoPos = new Vector2(30, 0);


    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        dialogueBox.SetActive(false);
        dialogueChoiceBox.SetActive(false);
        choiceArrow.anchoredPosition = arrowYesPos;
    }

    private void Start() {
        GameInput.Instance.OnUIContinuePressed += Instance_OnUIContinuePressed;
        GameInput.Instance.OnUISelectChoicePressed += Instance_OnUISelectChoicePressed;
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

    private void Instance_OnUIContinuePressed(object sender, System.EventArgs e) {
        if (dialogueChoiceActive) {
            HideDialogue();
            SelectChoice();
        }
        if (textDone) {
            if (lineIndex >= maxIndex) { // When all lines are complete and no choices
                HideDialogue();
            }
            else {
                dialogueActive = true; // Continue with next line
            }
            
        }
        else {
            textSpeed = 0f;
        }
    }

    private void Instance_OnUISelectChoicePressed(object sender, System.EventArgs e) {
        if (dialogueChoiceActive) {
            // Display choice arrow at selected choice
            if (choice == 0) { choice = 1; choiceArrow.anchoredPosition = arrowNoPos; }
            else { choice = 0; choiceArrow.anchoredPosition = arrowYesPos; }
        }
    }

    public void InitializeDialogue(string[] dialogueText, IHasDialogue dialogueInterface, bool hasChoice) {

        // Pause Player Input
        GameInput.Instance.PausePlayer();
        // Need to Add Pause Monster Movement

        // Initialize this scripts variables
        dialogueBox.SetActive(true);
        dialogueActive = true;
        dialogue = dialogueText;
        dialogueParent = dialogueInterface;
        this.hasChoice = hasChoice;

        // If line index is equal to max index, next continue will exit the dialogue
        lineIndex = 0;
        maxIndex = dialogueText.Length;
    }

    private void HideDialogue() {
        if (hasChoice) {
            // Reset dialogue choice variables
            dialogueChoiceActive = false;
            choiceArrow.anchoredPosition = arrowYesPos;
            dialogueChoiceBox.SetActive(false);
        }
        dialogueBox.SetActive(false);
        dialogueParent.DialogueDone = true;
        GameInput.Instance.UnPausePlayer();
    }



    private IEnumerator DisplayLine(string line) {
        foreach (char c in line.ToCharArray()) {
            // Add TMP logic here 
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        textDone = true;
        textSpeed = .05f;

        // After all strings from array have been displayed
        if (lineIndex >= maxIndex && hasChoice) {
            ShowChoice();
        }
    }

    private void ShowChoice() {
        dialogueChoiceBox.SetActive(true);
        dialogueChoiceActive = true;
    }

    private void SelectChoice() {
        // Choice is 0 for yes and 1 for no
        switch (choice) {
            case 0:
                // Reset to last save file
                choice = 0;
                SceneManager.LoadScene("CaveLevel1");
                break;
            case 1:
                choice = 0;
                SceneManager.LoadScene("MainMenu");
                break;
        }
    }



    private void EraseText() {
        dialogueText.text = "";
    }
}
