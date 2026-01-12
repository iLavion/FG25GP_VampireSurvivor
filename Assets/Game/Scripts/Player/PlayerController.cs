using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Stamina))]
public class PlayerController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float staminaCostPerSecond = 20f;
    private const float MIN_SPEED = 0f;
    private Vector2 moveInput;
    private bool isRunning;
    private Stamina stamina;
    private Keyboard keyboard;
    private void Start() {
        stamina = GetComponent<Stamina>();
        keyboard = Keyboard.current;
    }
    private void FixedUpdate() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        bool tryingToRun = keyboard != null && keyboard.shiftKey.isPressed && moveInput.sqrMagnitude > 0.01f;
        if (tryingToRun && stamina != null) { float staminaNeeded = staminaCostPerSecond * Time.fixedDeltaTime; isRunning = stamina.TryUse(staminaNeeded); }
        else isRunning = false;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 input = new(moveInput.x, 0f, moveInput.y);
        Vector3 movement = input.normalized * currentSpeed * Time.fixedDeltaTime;
        transform.position += movement;
    }
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void AddMoveSpeed(float delta) { walkSpeed = Mathf.Max(MIN_SPEED, walkSpeed + delta); runSpeed = Mathf.Max(MIN_SPEED, runSpeed + delta); }
    public void AddMoveSpeedPercent(float percent) { walkSpeed = Mathf.Max(MIN_SPEED, walkSpeed * (1f + percent)); runSpeed = Mathf.Max(MIN_SPEED, runSpeed * (1f + percent)); }
}
