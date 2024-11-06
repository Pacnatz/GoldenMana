using System;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

    public event EventHandler OnJumpPressed;
    public event EventHandler OnJumpCanceled;

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
        
    }

    private void Jump_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpPressed?.Invoke(this, EventArgs.Empty);
    }

    private void Jump_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnJumpCanceled?.Invoke(this, EventArgs.Empty);
    }

    public float GetMovementVectorX() {
        float moveDirX = playerInput.Player.XMovement.ReadValue<float>();

        return moveDirX;
    }

}
