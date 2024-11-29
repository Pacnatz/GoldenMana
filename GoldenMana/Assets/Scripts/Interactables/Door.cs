using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Door : BaseInteractable {

    [SerializeField] private string sceneToLoad;

    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed1;
    }

    private void Instance_OnInteractPressed1(object sender, EventArgs e) {
        if (hasPlayer) {

            // Save scene data

            // Load scene data
            SceneManager.LoadScene(sceneToLoad);
        }
    }

}
