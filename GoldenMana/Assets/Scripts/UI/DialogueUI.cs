using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance;

    [SerializeField] private GameObject dialogueBox;


    private float textSpeed = .4f;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        Instance = this;
    }

    public IEnumerator OpenUIDialogue(string[] dialogueText, IHasDialogue dialogueParent) {

        dialogueBox.SetActive(true);
        foreach (string text in dialogueText) {
            foreach (char c in text) {
                // Add TMP logic here 
                yield return new WaitForSeconds(textSpeed);
            }
        }
        dialogueBox.SetActive(false);
        dialogueParent.DialogueDone = true;

    }
}
