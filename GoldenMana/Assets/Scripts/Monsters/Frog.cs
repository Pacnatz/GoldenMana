using UnityEngine;
using System.Collections;

public class Frog : BaseMonster
{

    private Vector2 playerPos;

    private bool inRange = false;
    private bool onFloor = false;
    private bool idleOneshot = true;

    private float jumpCoolDown;

    private void Awake() {
        jumpCoolDown = Random.Range(1f, 2.5f);
        
        health = 4f;
        Damage = 10f;
    }

    protected override void Update() {
        base.Update();

        if (Player.Instance) {
            playerPos = Player.Instance.transform.position;
        }
        

        HandleSprite();
        CheckGround();

        if (inRange && onFloor) {
            jumpCoolDown -= Time.deltaTime;
            if (jumpCoolDown <= 0) {
                jumpCoolDown = Random.Range(1f, 2.5f);
                HandleJump();
            }
        }

        if (rb.linearVelocityY < -.1f) {
            anim.Play("Fall");
            idleOneshot = true;
        }

    }

    private void HandleSprite() {
        // Flipping sprite
        if (playerPos.x < transform.position.x) {
            sr.flipX = true;
        }
        else {
            sr.flipX = false;
        }
    }

    private void CheckGround() {
        float frogWidth = .45f;
        RaycastHit2D isGrounded = Physics2D.CircleCast(transform.position, frogWidth, Vector2.down, .35f, floorLayer);
        if (isGrounded) {
            onFloor = true;
            if (idleOneshot) {
                idleOneshot = false;
                anim.Play("Idle");
                rb.linearVelocityX = 0;
            }
        }
        else { onFloor = false; }
    }


    private void HandleJump() {
        float minJumpPower = 15;
        float maxJumpPower = 40;
        if (playerPos.x < transform.position.x) {
            rb.AddForce(new Vector2(-Random.Range(minJumpPower, maxJumpPower), Random.Range(minJumpPower, maxJumpPower)), ForceMode2D.Impulse);
        }
        else {
            rb.AddForce(new Vector2(Random.Range(minJumpPower, maxJumpPower), Random.Range(minJumpPower, maxJumpPower)), ForceMode2D.Impulse);
        }
        anim.Play("Jump");
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            inRange = false;
        }
    }
}
