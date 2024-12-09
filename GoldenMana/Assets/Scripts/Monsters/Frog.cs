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
        
        health = 4;
        Damage = 1;

        manaAmount = 3;
    }

    protected override void Update() {
        base.Update();

        if (Player.Instance) {
            playerPos = Player.Instance.transform.position;
        }


        GetXDirectionStatic();
        CheckGround();

        if (inRange && onFloor) {
            jumpCoolDown -= Time.deltaTime;
            if (jumpCoolDown <= 0) {
                jumpCoolDown = Random.Range(1f, 2.5f);
                HandleJump();
            }
        }

        if (onFloor) {
            rb.linearVelocityX = 0;
        }


        if (rb.linearVelocityY < -.1f && !onFloor) {
            anim.Play("Fall");
            idleOneshot = true;
        }

    }

    private bool GetXDirectionStatic() {
        // Flipping sprite
        if (playerPos.x < transform.position.x) {
            sr.flipX = true;
            return true;
        }
        else {
            sr.flipX = false;
            return false;
        }
        // Returns true if player is left side
    }

    private void CheckGround() {
        RaycastHit2D isGrounded = Physics2D.CircleCast(transform.position - new Vector3(0, .5f, 0), .4f ,Vector2.down, .2f, wallLayer);
        if (isGrounded) {
            rb.AddForceY(2f, ForceMode2D.Force);

            if (idleOneshot) {
                idleOneshot = false;
                onFloor = true;
                anim.Play("Idle");

            }
        } 
        else { onFloor = false; }
    }


    private void HandleJump() {
        onFloor = false;

        RaycastHit2D hitWall;
        RaycastHit2D hitWallFar;
        if (GetXDirectionStatic()) {
            hitWall = Physics2D.Raycast(transform.position - new Vector3(0, .5f, 0), Vector2.left, 1f, wallLayer);
            hitWallFar = Physics2D.Raycast(transform.position - new Vector3(0, .5f, 0), Vector2.left, 3.5f, wallLayer);
        }
        else {
            hitWall = Physics2D.Raycast(transform.position - new Vector3(0, .5f, 0), Vector2.right, 1f, wallLayer);
            hitWallFar = Physics2D.Raycast(transform.position - new Vector3(0, .5f, 0), Vector2.right, 3.5f, wallLayer);
        }

        if (hitWall) {
            if (GetXDirectionStatic()) {
                rb.AddForce(new Vector2(Random.Range(3, 10), Random.Range(25, 35)), ForceMode2D.Impulse);
            }
            else {
                rb.AddForce(new Vector2(-Random.Range(3, 10), Random.Range(25, 35)), ForceMode2D.Impulse);
            }
        }
        else if (hitWallFar) {
            if (GetXDirectionStatic()) {
                rb.AddForce(new Vector2(-Random.Range(15, 25), Random.Range(35f, 45f)), ForceMode2D.Impulse);
            }
            else {
                rb.AddForce(new Vector2(Random.Range(10, 20), Random.Range(35f, 45f)), ForceMode2D.Impulse);
            }


        }
        else {
            if (GetXDirectionStatic()) {
                rb.AddForce(new Vector2(-Random.Range(35, 40), Random.Range(10, 35)), ForceMode2D.Impulse);
            }
            else {
                rb.AddForce(new Vector2(Random.Range(35, 40), Random.Range(10, 35)), ForceMode2D.Impulse);
            }
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
