using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    public event EventHandler OnJumpPressed;
    public event EventHandler OnJumpCanceled;
    public event EventHandler OnInteractPressed;
    public event EventHandler OnUIContinuePressed;

    private PlayerInput playerInput;
    
    private void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
        }
        else {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }

    private void Start() {
        playerInput = new PlayerInput();

        playerInput.Player.Enable();

        playerInput.Player.Jump.performed += Jump_performed;
        playerInput.Player.Jump.canceled += Jump_canceled;
        playerInput.Player.Interact.performed += Interact_performed;

        playerInput.UI.Continue.performed += Continue_performed;
        
    }

    
    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpPressed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnInteractPressed?.Invoke(this, EventArgs.Empty);
    }

    public float GetMovementVectorX() {
        float moveDirX = playerInput.Player.XMovement.ReadValue<float>();

        return moveDirX;
    }

    public float GetAttackVectorX() {
        float attackX = playerInput.Player.AttackX.ReadValue<float>();
        return attackX;
    }
    public float GetAttackVectorY() {
        float attackY = playerInput.Player.AttackY.ReadValue<float>();
        return attackY;
    }

    public void PausePlayer() {
        playerInput.Player.Disable();
        playerInput.UI.Enable();
    }

    public void UnPausePlayer() {
        playerInput.UI.Disable();
        playerInput.Player.Enable();
        
    }

    private void Continue_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnUIContinuePressed?.Invoke(this, EventArgs.Empty);
    }


}
