using UnityEngine;

public class MinotaurShadow : MonoBehaviour
{
    private Minotaur minotaur;
    private SpriteRenderer sr;
    private Material mat;

    private float alphaValue;


    private void Awake() {
        sr = GetComponent<SpriteRenderer>();
        alphaValue = .3f;
        mat = sr.material;
        mat.SetFloat("_AlphaValue", alphaValue);

    }
    public void Initialize(Minotaur min) {
        minotaur = min;
        if (minotaur) {
            if (minotaur.GetDashDirection() == -1) {
                sr.flipX = true;
            }
            else {
                sr.flipX = false;
            }


        }
    }

    private void Update() {

        float fadeSpeed = 4f;
        alphaValue = Mathf.Lerp(alphaValue, 0, fadeSpeed * Time.deltaTime);

        mat.SetFloat("_AlphaValue", alphaValue);


        if (alphaValue < 0.03) {
            Destroy(gameObject);
        }
    }
}
