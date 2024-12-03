using UnityEngine;
using System.Collections;

public class SceneTransitions : MonoBehaviour {

    public static SceneTransitions Instance { get; private set; }

    [SerializeField] private Animator anim;


    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    public void SetBlack() {
        anim.Play("black");
    }

    public void StartScene() {
        StartCoroutine(StartSceneWithDelay(.5f));
    }

    public void EndScene() {
        anim.Play("end");
    }

    private IEnumerator StartSceneWithDelay(float sceneDelay) {
        yield return new WaitForSeconds(sceneDelay);
        anim.Play("start");
    }
}
