using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour {
    

    public void OnNewGameButtonPressed() {
        // Load tutorial
        SceneManager.LoadScene("CaveLevel1");

    }

    public void OnLoadGameButtonPressed() {
        // Load save states
    }
}
