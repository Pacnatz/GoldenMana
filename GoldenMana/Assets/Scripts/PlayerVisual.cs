using UnityEngine;
using Unity.Cinemachine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private CinemachineFollow cameraFollow;

    private SpriteRenderer sprite;
    private Animator anim;
    private float moveDir;
    private float moveDirStatic;
    
    private bool isJumping;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        PlayerMove.Instance.OnJumpAction += Instance_OnJumpAction;
    }

    

    private void Update() {
        moveDir = GameInput.Instance.GetMovementVectorX();

        if (moveDir != 0) {
            moveDirStatic = moveDir;
        }

        // Running logic
        if (moveDir > 0) {
            if (!isJumping && PlayerMove.Instance.onFloor) {
                anim.Play("Run");
            }
            sprite.flipX = false;
        }
        else if (moveDir < 0) {
            if (!isJumping && PlayerMove.Instance.onFloor) {
                anim.Play("Run");
            }
            sprite.flipX = true;
        }
        else {
            if (!isJumping && PlayerMove.Instance.onFloor) {
                anim.Play("Idle");
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
    }


    private void Instance_OnJumpAction(object sender, PlayerMove.OnJumpActionEventArgs e) {
        if (e.IsJumping) {
            anim.Play("Jump");
            isJumping = true;
        }
        else {
            if (!PlayerMove.Instance.onFloor) {
                anim.Play("Fall");
            }
            isJumping = false;
        }
    }
}
