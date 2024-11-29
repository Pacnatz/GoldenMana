using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public event EventHandler<OnAttackPressedEventArgs> OnAttackPressed;
    public class OnAttackPressedEventArgs : EventArgs {
        public Vector2 AttackDir;
    }

    [SerializeField] private GameObject fireballPrefab;

    private AttackMode attackState;
    private int attackIndex = 0;
    private int minIndex = 0;
    private int maxIndex = 0;

    private bool canFire;
    private int fireballsActive;

    private float attackX;
    private float attackY;


    private void Start() {
        attackState = AttackMode.None;
    }

    private void Update() {
        // Get attack vectors
        attackX = GameInput.Instance.GetAttackVectorX();
        attackY = GameInput.Instance.GetAttackVectorY();


        // Clamp attack index
        attackIndex = Mathf.Clamp(attackIndex, minIndex, maxIndex);
        attackState = (AttackMode)attackIndex;

        // Add more attackState cases
        switch (attackState) {
            case AttackMode.None:
                break;
            case AttackMode.FireSpell:
                HandleFireBallAttack();
                break;
        }
       
        
    }


    private void HandleFireBallAttack() {
        // canFire attack logic
        if (attackX == 0 && attackY == 0 && fireballsActive < 2) {
            canFire = true;
        }

        if (attackX != 0 && canFire) {
            // Handle X axis attack
            Vector3 xOffset = new Vector3(.5f, 0, 0);
            if (attackX > 0 && PlayerMove.Instance.GetMoveDirectionStaticX() > 0) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position + xOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(attackX, 0));
                }
                fireballsActive++;
                OnAttackPressed?.Invoke(this, new OnAttackPressedEventArgs { AttackDir = new Vector2(attackX, 0) });
            }
            else if (attackX < 0 && PlayerMove.Instance.GetMoveDirectionStaticX() < 0) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position - xOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(attackX, 0));
                }
                fireball.transform.Rotate(transform.forward, 180);
                fireballsActive++;
                OnAttackPressed?.Invoke(this, new OnAttackPressedEventArgs { AttackDir = new Vector2(attackX, 0) });
            }
            // Handle Cooldown
            canFire = false;
        }
        if (attackY != 0 && canFire) {
            // Handle Y axis attack
            Vector3 yOffset = new Vector3(0, .5f, 0);
            if (attackY > 0) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position + yOffset + new Vector3(0, .25f, 0), Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(0, attackY));
                }
                fireball.transform.Rotate(transform.forward, 90);
                fireballsActive++;
                OnAttackPressed?.Invoke(this, new OnAttackPressedEventArgs { AttackDir = new Vector2(0, attackY) });
            }
            else if (attackY < 0 && PlayerMove.Instance.onFloor == false) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position - yOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(0, attackY));
                }
                fireball.transform.Rotate(transform.forward, -90);
                fireballsActive++;
                OnAttackPressed?.Invoke(this, new OnAttackPressedEventArgs { AttackDir = new Vector2(0, attackY) });
            }
            // Handle Cooldown
            canFire = false;
        }
    }

    // Allows only 2 fire balls on the scene at once
    public void DeductFireBallsActive() {
        fireballsActive--;
    }

    // Unlocks firespell
    public void UnlockFireSpell() {
        minIndex = 1;
        maxIndex = 1;
    }

    public enum AttackMode {
        None,
        FireSpell,
        ThunderSpell,
        EarthSpell
    }

    
}
