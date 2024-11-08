using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseInteractable : MonoBehaviour
{

    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;


    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) {
        
    }

    protected virtual void Instance_OnInteractPressed(object sender, System.EventArgs e) {
        Debug.LogWarning("Interact called from baseInteractable");
    }
}
