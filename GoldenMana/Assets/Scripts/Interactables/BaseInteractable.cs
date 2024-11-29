using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseInteractable : MonoBehaviour
{
    protected bool hasPlayer = false;

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
