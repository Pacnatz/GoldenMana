using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    public event EventHandler OnJumpPressed;
    public event EventHandler OnJumpCanceled;
    public event EventHandler OnInteractPressed;

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

    public Vector2 GetAttackVector() {
        Vector2 attackVector = playerInput.Player.Attack.ReadValue<Vector2>();
        return attackVector;
    }

}
