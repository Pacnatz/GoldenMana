using UnityEngine;
using Unity.Cinemachine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private PlayerAttack attackScript;
    [SerializeField] private CinemachineFollow cameraFollow;
    [SerializeField] SpriteRenderer topSprite;
    [SerializeField] SpriteRenderer bottomSprite;

    [SerializeField] private Animator topAnim;
    [SerializeField] private Animator bottomAnim;
    
    private float moveDir;
    private float moveDirStatic;
    
    private bool isJumping;
    private bool isFalling;
    private bool isAttacking;
    private Vector2 attackDir;

    private float attackVectorX;

    private bool isFlashing = false;
    private float flashAmount;

    private void Start() {
        PlayerMove.Instance.OnJumpAction += PlayerInstance_OnJumpAction;
        PlayerMove.Instance.OnPlayerHit += Instance_OnPlayerHit;
        attackScript.OnAttackPressed += AttackScript_OnAttackPressed;
    }



    private void Update() {
        // Move direction
        moveDir = GameInput.Instance.GetMovementVectorX();
        moveDirStatic = PlayerMove.Instance.GetMoveDirectionStaticX();



        // Running logic
        
        if (moveDir > 0) {
            if (!isJumping && PlayerMove.Instance.onFloor) {
                bottomAnim.Play("Run");
            }
            topSprite.flipX = false;
            bottomSprite.flipX = false;
        }
        else if (moveDir < 0) {
            if (!isJumping && PlayerMove.Instance.onFloor) {
                bottomAnim.Play("Run");
            }
            topSprite.flipX = true;
            bottomSprite.flipX = true;
        }
        else {
            if (!isJumping && PlayerMove.Instance.onFloor) {
                bottomAnim.Play("Idle");
            }
        }

        // Attack Logic
        if (isAttacking) {
            if (attackDir.x != 0) {
                topAnim.Play("AttackSide");
            }
            if (attackDir.y > 0) {
                topAnim.Play("AttackTop");
            }
            if (attackDir.y < 0) {
                topAnim.Play("AttackBottom");
            }
        }
        else if (isJumping) {
            topAnim.Play("Jump");
        }
        else if (isFalling) {
            topAnim.Play("Fall");
        }
        else { // On floor
            if (moveDir > 0) {
                if (!isJumping && PlayerMove.Instance.onFloor) {
                    topAnim.Play("Run");
                }
                topSprite.flipX = false;
                bottomSprite.flipX = false;
            }
            else if (moveDir < 0) {
                if (!isJumping && PlayerMove.Instance.onFloor) {
                    topAnim.Play("Run");
                }
                topSprite.flipX = true;
                bottomSprite.flipX = true;
            }
            else {
                if (!isJumping && PlayerMove.Instance.onFloor) {
                    topAnim.Play("Idle");
                }
            }
        }

        // Camera Logic
        float offset = 2;
        float cameraVelocity = 2f;
        if (moveDirStatic > 0) {
            cameraFollow.FollowOffset.x = Mathf.MoveTowards(cameraFollow.FollowOffset.x, offset, cameraVelocity * Time.deltaTime);
        }
        else if (moveDirStatic < 0) {
            cameraFollow.FollowOffset.x = Mathf.MoveTowards(cameraFollow.FollowOffset.x, -offset, cameraVelocity * Time.deltaTime);
        }
        else {
            cameraFollow.FollowOffset.x = Mathf.MoveTowards(cameraFollow.FollowOffset.x, 0, cameraVelocity * Time.deltaTime);
        }

        // Flash Logic
        if (isFlashing) {
            HandleFlash();
        }
    }


    private void AttackScript_OnAttackPressed(object sender, PlayerAttack.OnAttackPressedEventArgs e) {
        if (isAttacking) { // If already attacking
            return;
        }
        isAttacking = true;
        attackDir = e.AttackDir;
        float attackAnimTime = .1f;
        Invoke(nameof(TurnAttackOff), attackAnimTime);
    }

    private void TurnAttackOff() {
        isAttacking = false;
    }

    private void PlayerInstance_OnJumpAction(object sender, PlayerMove.OnJumpActionEventArgs e) {
        if (topAnim != null && bottomAnim != null) { // Test here or bad things will happen
            if (e.IsJumping) {
                isJumping = true;
                bottomAnim.Play("Jump");
                topAnim.Play("Jump");
            }
            else {
                if (!PlayerMove.Instance.onFloor) {
                    bottomAnim.Play("Fall");
                    isFalling = true;
                }
                else {
                    isFalling = false;
                }
                isJumping = false;
            }
        }
        
    }

    // Handle flash
    private void Instance_OnPlayerHit(object sender, PlayerMove.OnPlayerHitEventArgs e) {
        isFlashing = true;
        flashAmount = 1f;
    }

    private void HandleFlash() {
        float flashRate = 50f;
        Material topMat = topSprite.material;
        Material bottomMat = bottomSprite.material;
        topMat.SetColor("_FlashColor", Color.red);
        bottomMat.SetColor("_FlashColor", Color.red);
        flashAmount = Mathf.Lerp(flashAmount, 0, flashRate * Time.deltaTime);
        topMat.SetFloat("_FlashAmount", flashAmount);
        bottomMat.SetFloat("_FlashAmount", flashAmount);
        if (flashAmount <= 0.05) {
            topMat.SetFloat("_FlashAmount", 0);
            bottomMat.SetFloat("_FlashAmount", 0);
            isFlashing = false;
        }
    }
}
