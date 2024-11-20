using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseInteractable : MonoBehaviour
{
    protected bool hasPlayer = false;
    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;

    }

    protected virtual void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        Debug.LogWarning("Interact called from baseInteractable");
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            hasPlayer = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            hasPlayer = false;
        }
    }
}
