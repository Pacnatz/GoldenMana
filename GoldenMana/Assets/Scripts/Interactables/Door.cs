using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Door : BaseInteractable {

    [SerializeField] private GameObject lockSprite;
    public bool isLocked;
    [SerializeField] private string sceneToLoad;
    [SerializeField] private Vector2 playerLoadPos;
    private void Start() {
        GameInput.Instance.OnInteractPressed += Instance_OnInteractPressed;


    }

    protected virtual void Update() {
        if (isLocked) {
            lockSprite.SetActive(true);
        }
        else {
            lockSprite.SetActive(false);
        }
    }



    private void Instance_OnInteractPressed(object sender, EventArgs e) {
        if (hasPlayer && !isLocked) {
            // Load Scene
            // Set player position
            if (SceneTransitions.Instance) {
                SceneTransitions.Instance.EndScene();
            }
            
            StartCoroutine(LoadNextScene(.5f));
        }

    }

    private IEnumerator LoadNextScene(float sceneDelay) {
        yield return new WaitForSeconds(sceneDelay);
        SaveManager.Instance.LoadSceneFromDoor(sceneToLoad, playerLoadPos);
    }
}
