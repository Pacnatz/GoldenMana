using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject fireballPrefab;

    //Attack Spawn Positions
    [SerializeField] private Transform leftAttackPos;
    [SerializeField] private Transform rightAttackPos;
    [SerializeField] private Transform topAttackPos;
    [SerializeField] private Transform bottomAttackPos;

    private Transform attackPos;

    private Vector2 attackDir;


    private void Start() {
        
    }

    private void Update() {
        attackDir = GameInput.Instance.GetAttackVector();
        if (attackDir != Vector2.zero) {
            HandleAttack();
        }
        
        
    }

    private void HandleAttack() {
        if (attackDir.x > 0) {
            attackPos = rightAttackPos;
            Instantiate(fireballPrefab, attackPos.position, Quaternion.identity).TryGetComponent<Fireball>(out var fireball);
            fireball.SetInitialVelocity(Vector2.right);
            
        }
        else if (attackDir.x < 0) {
            attackPos = leftAttackPos;
            Instantiate(fireballPrefab, attackPos.position, Quaternion.identity).TryGetComponent<Fireball>(out var fireball);
            fireball.SetInitialVelocity(Vector2.left);

        }


    }
}
