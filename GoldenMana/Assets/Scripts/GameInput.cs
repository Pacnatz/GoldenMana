using UnityEngine;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance;

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

        playerInput.Player.XMovement.Enable();
        
    }

    public float GetMovementVectorX() {
        float moveDirX = playerInput.Player.XMovement.ReadValue<float>();

        return moveDirX;
    }

}
