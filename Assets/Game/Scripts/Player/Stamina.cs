using UnityEngine;
using UnityEngine.Events;

public class Stamina : MonoBehaviour {
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina = 100f;
    [SerializeField] private float regenRate = 10f;
    [SerializeField] private float regenDelay = 1f;
    public UnityEvent<float, float> onStaminaChanged;
    private float lastUseTime;
    private void Awake() {
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
    private void Update() {
        if (currentStamina < maxStamina && Time.time >= lastUseTime + regenDelay) {
            currentStamina = Mathf.Min(maxStamina, currentStamina + regenRate * Time.deltaTime);
            onStaminaChanged?.Invoke(currentStamina, maxStamina);
        }
    }
    public bool TryUse(float amount) {
        if (currentStamina < amount) return false;
        currentStamina -= amount;
        lastUseTime = Time.time;
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
        return true;
    }
    public void SetMaxStamina(float value, bool refill = true) {
        maxStamina = Mathf.Max(1f, value);
        if (refill) currentStamina = maxStamina;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        onStaminaChanged?.Invoke(currentStamina, maxStamina);
    }
    public float Current => currentStamina;
    public float Max => maxStamina;
}
