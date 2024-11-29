using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Door : BaseInteractable {

    [SerializeField] private string sceneToLoad;
    [SerializeField] private Vector2 playerLoadPos;
    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;
    }

    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        if (hasPlayer) {
            // Load Scene
            // Set player position
            SaveManager.Instance.LoadSceneFromDoor(sceneToLoad, playerLoadPos);
        }

    }

}
