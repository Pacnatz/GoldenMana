using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour {

    public static PlayerUI Instance;

    [SerializeField] private GameObject[] hearts;
    [SerializeField] private Sprite[] heartSprites;

    private int numHearts;
    private int health;
    

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        // Disable each heart on start
        foreach (GameObject heart in hearts) {
            heart.SetActive(false);
        }
    }

    private void Update() {

        health = Player.Instance.health;
        numHearts = Player.Instance.maxHealth / 4;



        for (int i = 0; i < numHearts; i++) {
            if (SceneManager.GetActiveScene().name != "MainMenu") {
                hearts[i].SetActive(true);
            }
            else {
                hearts[i].SetActive(false);
            }



            if (health >= 4) {
                hearts[i].GetComponent<Image>().sprite = heartSprites[4];
                health -= 4;
            }
            else if (health > 0) {
                hearts[i].GetComponent<Image>().sprite = heartSprites[health];
                health = 0;
            }
            else {
                hearts[i].GetComponent<Image>().sprite = heartSprites[0];
            }
            
        }

    }


}
