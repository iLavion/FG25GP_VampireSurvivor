using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyBoss : EnemyBase {
    [SerializeField] private float bossDamageScale = 2f;
    [SerializeField] private float bossHealthScale = 1f;
    [SerializeField] private float bossDifficultyScale = 1.5f;
    [SerializeField] private float shootInterval = 1.5f;
    [SerializeField] private float projectileDamage = 15f;
    [SerializeField] private LayerMask projectileHitLayers;
    private float shootTimer;
    private ProjectilePool projectilePool;
    private PlayerCombat playerCombat;
    private float baseMoveSpeed;
    private float baseDamage;
    private float baseMaxHealth;
    protected override void InitializeEnemyType() => Type = EnemyType.Boss;
    protected override void Awake() {
        base.Awake();
        baseMoveSpeed = moveSpeed;
        baseDamage = damage;
        if (TryGetComponent<Health>(out var hpInit)) baseMaxHealth = hpInit.Max;
    }
    protected override void Start() {
        base.Start();
        if (TryGetComponent<Health>(out var hp)) {
            float mult = difficultyMultiplier;
            hp.SetMaxHealth(baseMaxHealth * bossHealthScale * mult, refill: true);
        }
        shootTimer = shootInterval;
        projectilePool = FindFirstObjectByType<ProjectilePool>();
        playerCombat = FindFirstObjectByType<PlayerCombat>();
    }
    protected override void OnEnable() {
        base.OnEnable();
        moveSpeed = baseMoveSpeed * bossDifficultyScale * difficultyMultiplier;
        damage = baseDamage * bossDamageScale;
        var hp = GetComponent<Health>();
        if (hp != null) {
            float mult = difficultyMultiplier;
            hp.SetMaxHealth(baseMaxHealth * bossHealthScale * mult, refill: true);
        }
    }
    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        if (player == null || projectilePool == null || playerCombat == null) return;
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f) { ShootAtPlayer(); shootTimer = shootInterval; }
    }
    private void ShootAtPlayer() {
        Vector3 dir = (player.position - transform.position).normalized;
        var proj = projectilePool.Get();
        if (proj != null) {
            proj.transform.position = transform.position + dir * 1f;
            proj.Init(dir, playerCombat.ProjectileSpeed, projectileDamage, playerCombat.ProjectileLifetime, projectileHitLayers, projectilePool, gameObject);
        }
    }
    protected override Vector3 GetMoveDirection() {
        if (player == null) return Vector3.zero;
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        return toPlayer.normalized;
    }
}
