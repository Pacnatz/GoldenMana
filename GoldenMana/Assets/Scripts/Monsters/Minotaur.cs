using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;


public class Minotaur : BaseMonster, IHasDialogue {

    public event EventHandler OnDamageTaken;

    [SerializeField] private GameObject door;

    private bool isActive = false;

    // Dialogue variables
    private bool canStart = true;
    public string[] Dialogue { get; set; }
    public bool HasChoice { get; set; }

    private Vector2 startPos;
    private Vector2 playerPos;
    private LayerMask playerLayer;
    private bool onFloor;

    private int maxHealth;
    private bool isDead = false;

    private bool isAttacking;
    private bool attackOneShot;
    private float idleTimer;
    private int attackPhase;

    // Attack variables
    // Dash variables
    private bool isDashing;
    private bool isKnockback;
    private int dashDirection;
    [SerializeField] GameObject minotaurShadow;
    private int dashCounter = 0;
    private float dashTimer = 0;

    // Jump variables
    private bool isJumping;
    private bool isFalling;
    private bool jumpOneShot;
    private int jumpDirection;
    private bool onFloorOneShot = true;

    private void Awake() {
        // Initialization
        isBossMonster = true;
        health = 200;
        Damage = 2;
        Dialogue = new string[] { "Prepare to die!" };
        playerLayer = LayerMask.GetMask("Player");
    }

    protected override void Start() {
        base.Start();
        startPos = transform.position;
        StartCoroutine(StartWithDelay(.3f));
        foreach (Vector2 enemyKilled in SaveManager.Instance.selectedScene.EnemyKillPos) {
            if (Vector2.Distance(enemyKilled, startPos) < 1) {
                canStart = false;
                door.GetComponent<Door>().isLocked = false;
                Destroy(gameObject);
            }
        }
        maxHealth = health;
    }
    private IEnumerator StartWithDelay(float delay) {


        yield return new WaitForSeconds(delay);

        if (canStart) {
            ((IHasDialogue)this).StartDialogue();
        }

    }

    protected override void Update() {

        if (!isAttacking) {
            //Flip sprite
            if (GetXDirectionStatic()) {
                sr.flipX = true;
            }
            else {
                sr.flipX = false;
            }
            float slowDownSpeed = 3f;
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, Time.deltaTime * slowDownSpeed);
        }
        
        if (isActive) {
            base.Update();


            onFloor = CheckGround();


            if (Player.Instance) {
                playerPos = Player.Instance.transform.position;
            }

            if (attackOneShot) {
                attackOneShot = false;
                isAttacking = true;
                attackPhase = Random.Range(1, 3);
                //attackPhase = 1;
                if (attackPhase == 2) {
                    jumpOneShot = true;
                }

            }

            switch (attackPhase) {
                case 0:
                    HandleIdle();
                    break;
                case 1:
                    HandleDashAttack();
                    break;
                case 2:
                    HandleJumpAttack();
                    break;
            }


            // Death logic
            if (health <= 0 && !isDead) {
                isDead = true;
                isActive = false;
                material.SetFloat("_FlashAmount", 0);
                Dialogue = new string[] { "You have bested me.", "Here's your key." };
                ((IHasDialogue)this).StartDialogue();
            }
        }
    }



    private void HandleIdle() {
        if (!isAttacking) {
            idleTimer -= Time.deltaTime;
            // Invoke animation
            anim.Play("Idle");
            if (idleTimer <= 0) {
                isAttacking = true;
                attackOneShot = true;
                
            }
        }
    }

    private void HandleDashAttack() {
        if (!isDashing) {
            anim.Play("Charge");

            if (isKnockback) {
                RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, 1.8f, wallLayer);
                if (hitGround) {
                    ResetAttack();
                }
            }
        }
        else {
            anim.Play("Dash");
            float dashSpeed = 10f;

            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0) {
                dashTimer = .2f;
                GameObject minShadow = Instantiate(minotaurShadow, transform.position, Quaternion.identity);
                minShadow.GetComponent<MinotaurShadow>().Initialize(this);
            }


            RaycastHit2D hitWall;
            if (dashDirection == -1) {
                rb.linearVelocityX = -dashSpeed;
                hitWall = Physics2D.Raycast(transform.position, Vector2.left, 1f, wallLayer);
            }
            else {
                rb.linearVelocityX = dashSpeed;
                hitWall = Physics2D.Raycast(transform.position, Vector2.right, 1f, wallLayer);
            }
            // Ending attack
            if (hitWall) {
                dashTimer = .2f;
                isDashing = false;
                StartCoroutine(DashKnockBack());
            }
            
        }
    }

    public void Dash() { // Called from MinotaurAnimationEvents
        dashCounter++;
        if (dashCounter >= 3) {
            dashCounter = 0;
            if (GetXDirectionStatic()) {
                dashDirection = -1;
            }
            else {
                dashDirection = 1;
            }
            
            isDashing = true;


        }

    }

    private IEnumerator DashKnockBack() {
        yield return new WaitForSeconds(.1f);
        rb.AddForce(new Vector2(200 * -dashDirection, 200), ForceMode2D.Impulse);
        yield return new WaitForSeconds(.3f);
        isKnockback = true;
    }

    private void HandleJumpAttack() {
        float jumpSpeed = 8;
        if (jumpOneShot) {
            jumpOneShot = false;
            isJumping = true;
            if (GetXDirectionStatic()) {
                jumpDirection = -1;
            }
            else {
                jumpDirection = 1;
            }
        }
        if (isJumping) {
            anim.Play("Jump");
            RaycastHit2D hitCeiling, hitWall, hitPlayer;
            hitCeiling = Physics2D.Raycast(transform.position, Vector2.up, .8f, wallLayer);
            hitPlayer = Physics2D.Raycast(transform.position, Vector2.down, 100f, playerLayer);

            if (jumpDirection == -1) {
                rb.linearVelocity = new Vector2(-1.4f * jumpSpeed, 1f * jumpSpeed);
                hitWall = Physics2D.Raycast(transform.position, Vector2.left, 1f, wallLayer);
            }
            else {
                rb.linearVelocity = new Vector2(1.4f * jumpSpeed, 1f * jumpSpeed);
                hitWall = Physics2D.Raycast(transform.position, Vector2.right, 1f, wallLayer);
            }

            if (hitCeiling || hitWall || hitPlayer) {
                isJumping = false;
                isFalling = true;
                rb.linearVelocity = Vector2.zero;
                //rb.linearVelocity = new Vector2(0, -15);
                // One shot to change to statue
                Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
                anim.Play("Statue");
            }
        }
        else {
            
            if (onFloor && onFloorOneShot) {
                onFloorOneShot = false;
                Instantiate(deathParticlesPrefab, transform.position - new Vector3(0, 1.6f, 0), Quaternion.identity);
                isFalling = false;
                StartCoroutine(ResetDelay());
            }
        }
    }

    private IEnumerator ResetDelay() {
        yield return new WaitForSeconds(.4f);
        ResetAttack();
        onFloorOneShot = true;
    }

    public bool GetIsFalling() => isFalling;
    public int GetDashDirection() => dashDirection;
    public int GetFallDirection() => jumpDirection;

    private void ResetAttack() {
        isAttacking = false;
        isKnockback = false;
        idleTimer = Random.Range(2, 3);
        attackPhase = 0;
    }

    private bool CheckGround() => Physics2D.Raycast(transform.position, Vector2.down, 1.5f, wallLayer);

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForceX(jumpDirection * 5, ForceMode2D.Impulse);
        }
    }

    private bool GetXDirectionStatic() {
        // Flipping sprite
        if (playerPos.x < transform.position.x) {
            return true;
        }
        else {
            return false;
        }
        // Returns true if player is left side
    }

    public override void TakeDamage(int damage) {
        base.TakeDamage(damage);
        OnDamageTaken?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealth() => health;
    public int GetMaxHealth() => maxHealth;

    public void DialogueDone() {
        isActive = true;
        attackOneShot = true;
        isAttacking = false;
        idleTimer = 1f;

        if (health <= 0 && canStart) {
            SaveManager.Instance.selectedScene.EnemyKillPos.Add(startPos);

            for (int i = 0; i < 5; i++) {
                GameObject deathParticles = Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
                Destroy(deathParticles, 1f);
            }
            Player.Instance.minotaurKilled = true;
            door.GetComponent<Door>().isLocked = false;
            Destroy(gameObject);
        }
    }
}
