using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerUI : MonoBehaviour {

    public static PlayerUI Instance;

    [SerializeField] private GameObject[] hearts;
    [SerializeField] private Sprite[] heartSprites;
    [SerializeField] private GameObject manaBar;
    [SerializeField] private Image manaFillBar;
    [SerializeField] private TextMeshProUGUI manaLevelField;

    private int numHearts;
    private int health;

    private bool isFlashing;
    private float flashAmount = 0;

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

        GetComponent<Canvas>().worldCamera = Camera.main;

        health = Player.Instance.health;
        numHearts = Player.Instance.maxHealth / 4;

        manaLevelField.text = $"Level : {Player.Instance.manaLevel}";

        // Don't show UI for these scenes
        for (int i = 0; i < numHearts; i++) {
            if (SceneManager.GetActiveScene().name != "MainMenu" || SceneManager.GetActiveScene().name == "CaveLevel4") {
                hearts[i].SetActive(true);
                manaBar.SetActive(true);
            }
            else {
                hearts[i].SetActive(false);
                manaBar.SetActive(false);
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


        // Handle mana visuals
        int mana = Player.Instance.mana;
        int manaLevel = Player.Instance.manaLevel;
        float fillTime = 6f;
        switch (manaLevel) {
            case 1:
                manaFillBar.fillAmount = Mathf.Lerp(manaFillBar.fillAmount, mana / 5f, fillTime * Time.deltaTime); // Update 5 to maxMana1
                break;
            case 2:
                manaFillBar.fillAmount = Mathf.Lerp(manaFillBar.fillAmount, mana / 10f, fillTime * Time.deltaTime);
                break;
            case 3:
                manaFillBar.fillAmount = Mathf.Clamp(mana / 15f, 0, 1);
                break;
        }


        if (isFlashing) {
            float flashRate = 10f;
            Material fillBarMat = manaFillBar.material;
            flashAmount = Mathf.Lerp(flashAmount, 0, flashRate * Time.deltaTime);
            fillBarMat.SetFloat("_FlashAmount", flashAmount);
            if (flashAmount <= 0.05) {
                flashAmount = 0;
                fillBarMat.SetFloat("_FlashAmount", 0);
                isFlashing = false;
            }
        }
    }


    public void HandleFlash() {  // Called from ManaPickup.cs
        flashAmount = 1;
        isFlashing = true;
    }



}
