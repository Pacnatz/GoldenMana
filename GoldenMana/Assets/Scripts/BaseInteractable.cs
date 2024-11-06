using UnityEngine;

public class BaseInteractable : MonoBehaviour
{

    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;
    
    }

    protected virtual void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        Debug.LogWarning("Interact called from baseInteractable");
    }
}
