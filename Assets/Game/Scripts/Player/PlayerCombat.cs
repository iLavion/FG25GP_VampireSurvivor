using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour {
    [Header("Combat")]
    [SerializeField] private float attackCooldown = 0.2f;
    [SerializeField] private float attackDamage = 25f;
    [SerializeField] private LayerMask enemyLayer;
    [Header("Projectile")]
    [SerializeField] private float projectileSpeed = 18f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private Vector3 spawnOffset = new(0f, 0.5f, 0f);
    public float ProjectileSpeed => projectileSpeed;
    public float ProjectileLifetime => projectileLifetime;
    private const float MIN_COOLDOWN_MULTIPLIER = 0.1f;
    private const float AIM_RAYCAST_DISTANCE = 2f;
    private const float AIM_DIRECTION_MIN_SQRMAG = 0.001f;
    private float nextAttackTime;
    private float attackCooldownMultiplier = 1f;
    private Camera mainCamera;
    private Mouse mouseCurrent;
    private ProjectilePool projectilePool;
    private void Start() {
        projectilePool = FindFirstObjectByType<ProjectilePool>();
        mainCamera = Camera.main;
        mouseCurrent = Mouse.current;
        RecalculateProjectilePool();
    }
    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        if (mouseCurrent != null && mouseCurrent.leftButton.isPressed) TryAttack();
    }
    private void RecalculateProjectilePool() {
        if (projectilePool == null) return;
        float effectiveAttackCooldown = attackCooldown * attackCooldownMultiplier;
        projectilePool.RecalculatePoolSize(effectiveAttackCooldown, projectileLifetime);
    }
    public void TryAttack() {
        if (Time.time < nextAttackTime) return;
        nextAttackTime = Time.time + attackCooldown * attackCooldownMultiplier;
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySoundEffect("PlayerAttack");
        Vector3 shootDir = GetAimDirection();
        Projectile proj;
        if (projectilePool != null) {
            proj = projectilePool.Get();
            if (proj != null) {
                proj.transform.SetPositionAndRotation(transform.position + spawnOffset, Quaternion.LookRotation(shootDir, Vector3.up));
                proj.Init(shootDir, projectileSpeed, attackDamage, projectileLifetime, enemyLayer, projectilePool, gameObject);
                return;
            }
        }
    }
    private Vector3 GetAimDirection() {
        if (mainCamera != null && mouseCurrent != null) {
            Ray ray = mainCamera.ScreenPointToRay(mouseCurrent.position.ReadValue());
            Plane plane = new(Vector3.up, new Vector3(0f, transform.position.y, 0f));
            if (plane.Raycast(ray, out float enter)) {
                Vector3 hit = ray.origin + ray.direction * enter;
                Vector3 dir = hit - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > AIM_DIRECTION_MIN_SQRMAG) return dir.normalized;
            }
        }
        Vector3 forward = new(transform.forward.x, 0f, transform.forward.z);
        return forward.sqrMagnitude > AIM_DIRECTION_MIN_SQRMAG ? forward.normalized : Vector3.forward;
    }
    public void AddDamage(float delta) => attackDamage = Mathf.Max(0f, attackDamage + delta);
    public void AddDamagePercent(float percent) => attackDamage = Mathf.Max(0f, attackDamage * (1f + percent));
    public void ModifyAttackCooldownPercent(float deltaPercent) {
        attackCooldownMultiplier = Mathf.Max(MIN_COOLDOWN_MULTIPLIER, attackCooldownMultiplier * (1f + deltaPercent));
        RecalculateProjectilePool();
    }
}
