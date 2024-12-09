using UnityEngine;
using System.Collections;

public class BaseMonster : MonoBehaviour {

    [SerializeField] private GameObject healthPickup;
    [SerializeField] private GameObject manaPickup;
    [SerializeField] protected GameObject deathParticlesPrefab;
    [SerializeField] protected Animator anim;

    protected bool isBossMonster = false;

    // Flash variables
    private float flashAmount = 1;
    protected Material material;
    private bool isFlashing = false;

    protected int health;
    [HideInInspector] public int Damage;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;


    protected LayerMask floorLayer;
    protected LayerMask wallLayer;

    protected int manaAmount;

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        material = sr.material;
        floorLayer = LayerMask.GetMask("Floor", "Breakable", "Monster");
        wallLayer = LayerMask.GetMask("Floor", "Breakable");
    }

    protected virtual void Update() {

        // Death logic
        if (health <= 0 && !isBossMonster) {

            for (int i = 0; i < manaAmount; i++) {
                Instantiate(manaPickup, transform.position, Quaternion.identity);
            }

            int chance = Random.Range(1, 101);
            if (chance < 25) {
                Instantiate(healthPickup, transform.position, Quaternion.identity);
            }

            GameObject deathParticles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(deathParticles, 1f);
            Destroy(gameObject);
        }

        // Flash logic
        if (isFlashing) {
            HandleFlash();
        }
        
    }


    public virtual void TakeDamage(int damage) {
        health -= damage;
        isFlashing = true;
        flashAmount = 1;
    }

    private void HandleFlash() {
        float flashRate = 50f;
        flashAmount = Mathf.Lerp(flashAmount, 0, flashRate * Time.deltaTime);
        material.SetFloat("_FlashAmount", flashAmount);
        if (flashAmount <= 0.05) {
            material.SetFloat("_FlashAmount", 0);
            isFlashing = false;
        }
    }
}
