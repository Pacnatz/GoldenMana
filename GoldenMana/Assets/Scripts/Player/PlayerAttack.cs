using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public event EventHandler<OnManaUsedEventArgs> OnManaUsed;
    public class OnManaUsedEventArgs : EventArgs {
        public float mana;
        public float timeToContinueRegenerateMana; // After a spell is cast, theres a delay until mana can regenerate
    }

    [SerializeField] private GameObject fireballPrefab;

    private AttackMode attackState;

    private bool canFire;
    private int fireballsActive;

    private float attackX;
    private float attackY;

    private void Start() {
        attackState = AttackMode.FireSpell;
    }

    private void Update() {

        // Get attack vectors
        attackX = GameInput.Instance.GetAttackVectorX();
        attackY = GameInput.Instance.GetAttackVectorY();

        // Add more attackState cases
        switch (attackState) {
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
            if (attackX > 0 && PlayerMove.Instance.GetMoveDirectionStaticX() >= 0) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position + xOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(attackX, 0));
                }
                fireballsActive++;
            }
            else if (attackX < 0 && PlayerMove.Instance.GetMoveDirectionStaticX() <= 0) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position - xOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(attackX, 0));
                }
                fireballsActive++;
            }
            // Handle Cooldown
            canFire = false;
            UseMana(1f);
        }
        if (attackY != 0 && canFire) {
            // Handle Y axis attack
            Vector3 yOffset = new Vector3(0, .5f, 0);
            if (attackY > 0) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position + yOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(0, attackY));
                }
                fireball.transform.Rotate(transform.forward, -90);
                fireballsActive++;
            }
            else if (attackY < 0 && PlayerMove.Instance.onFloor == false) {
                GameObject fireball = Instantiate(fireballPrefab, transform.position - yOffset, Quaternion.identity);
                if (fireball.TryGetComponent<Fireball>(out var fireballScript)) {  // If fireball script is attached
                    fireballScript.InitializeFireball(this, new Vector2(0, attackY));
                }
                fireball.transform.Rotate(transform.forward, -90);
                fireballsActive++;
            }
            // Handle Cooldown
            canFire = false;
            UseMana(1f);
        }
    }

    public void DeductFireBallsActive() {
        fireballsActive--;
    }

    private void UseMana(float manaConsumption) {
        OnManaUsed?.Invoke(this, new OnManaUsedEventArgs { mana = manaConsumption, timeToContinueRegenerateMana = .5f });
    }

    public enum AttackMode {
        FireSpell,
        ThunderSpell,
        EarthSpell
    }
}
