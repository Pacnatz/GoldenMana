using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour {

    [SerializeField] private GameObject creditsPanel;

    private void Start() {
        if (creditsPanel) {
            creditsPanel.SetActive(false);
        }
    }
    public void OnNewGameButtonPressed() {
        // Load tutorial
        SceneManager.LoadScene("CaveLevel1");

    }

    public void OnLoadGameButtonPressed() {
        // Load save states
        SaveManager.Instance.Load();
    }

    public void OnDeleteSaveButtonPressed() {
        // Delete save files
        SaveManager.Instance.DeleteSaveFile();
    }

    public void OnOpenCreditsPanel() {
        creditsPanel.SetActive(true);
    }

    public void OnCloseCreditsPanel() {
        creditsPanel.SetActive(false);
    }
}
