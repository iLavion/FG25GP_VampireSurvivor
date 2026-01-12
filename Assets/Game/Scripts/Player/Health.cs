using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;
    public UnityEvent<float, float> onHealthChanged;
    public UnityEvent onDeath;
    private bool isDead;
    private bool isEnemy;
    private void Awake() {
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        isEnemy = GetComponent<EnemyBase>() != null;
    }
    private void OnEnable() {
        isDead = false;
        if (currentHealth <= 0f) { currentHealth = maxHealth; onHealthChanged?.Invoke(currentHealth, maxHealth); }
    }
    public void TakeDamage(float amount) {
        if (isDead) return;
        float actualDamage = Mathf.Max(0f, amount);
        currentHealth = Mathf.Max(0f, currentHealth - actualDamage);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
        if (isEnemy) {
            var enemy = GetComponent<EnemyBase>();
            if (enemy != null) enemy.ShowDamage(actualDamage);
        } else { if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.15f, 0.2f); }
        if (currentHealth <= 0f) {
            isDead = true;
            if (!isEnemy && AudioManager.Instance != null) AudioManager.Instance.PlaySoundEffect("PlayerDeath");
            onDeath?.Invoke();
        }
    }
    public void Heal(float amount) {
        if (isDead) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Max(0f, amount));
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void SetMaxHealth(float value, bool refill = true) {
        maxHealth = Mathf.Max(1f, value);
        if (refill) currentHealth = maxHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public float Current => currentHealth;
    public float Max => maxHealth;
}
