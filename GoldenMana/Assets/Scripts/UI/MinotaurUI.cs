using UnityEngine;
using UnityEngine.UI;

public class MinotaurUI : MonoBehaviour {

    [SerializeField] private Image healthBar;
    [SerializeField] private Minotaur minotaur;

    private bool isFlashing;
    private float flashAmount;

    private void Start() {
        minotaur.OnDamageTaken += Minotaur_OnDamageTaken;
    }


    private void Update() {

        float fillTime = 6f;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, minotaur.GetHealth() / (float)minotaur.GetMaxHealth(), fillTime * Time.deltaTime);

        if (isFlashing) {
            float flashRate = 10f;
            Material healthBarMat = healthBar.material;
            flashAmount = Mathf.Lerp(flashAmount, 0, flashRate * Time.deltaTime);
            healthBarMat.SetFloat("_FlashAmount", flashAmount);
            if (flashAmount <= 0.05) {
                flashAmount = 0;
                healthBarMat.SetFloat("_FlashAmount", 0);
                isFlashing = false;
            }
        }
    }

    private void Minotaur_OnDamageTaken(object sender, System.EventArgs e) {
        isFlashing = true;
        flashAmount = 1;
    }
}
