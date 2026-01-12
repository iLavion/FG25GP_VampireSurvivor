using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public abstract class EnemyBase : MonoBehaviour {
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float attackRange = 1.5f;
    [SerializeField] protected float attackCooldown = 1.0f;
    protected Transform player;
    protected Rigidbody rb;
    protected float nextAttack;
    protected float difficultyMultiplier = 1f;
    protected EnemyPool pool;
    protected EnemySpawner spawner;
    protected Health health;
    protected Health playerHealth;
    protected int spawnedAtWave = 1;
    private EnemyHealth healthDisplay;
    private DamagePopup damagePopup;
    public EnemyType Type { get; protected set; }

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        health = GetComponent<Health>();
        InitializeEnemyType();
    }
    protected virtual void InitializeEnemyType() => Type = EnemyType.Chaser;
    protected virtual void OnEnable() {
        nextAttack = 0f;
        if (health != null) { health.onDeath.RemoveListener(OnDeath); health.onDeath.AddListener(OnDeath); }
        if (player == null) {
            var p = FindFirstObjectByType<PlayerController>();
            if (p != null) { player = p.transform; playerHealth = p.GetComponent<Health>(); }
        }
        CreateHealthDisplay();
        CreateDamagePopup();
    }
    protected virtual void Start() { }
    private void CreateHealthDisplay() {
        if (health == null) return;
        if (healthDisplay == null && EnemyHealthPool.Instance != null) healthDisplay = EnemyHealthPool.Instance.Get();
        if (healthDisplay != null) { healthDisplay.Initialize(health, transform); healthDisplay.gameObject.SetActive(true); }
    }
    
    private void CreateDamagePopup() {
        if (damagePopup == null && DamagePopupPool.Instance != null) damagePopup = DamagePopupPool.Instance.Get();
        if (damagePopup != null) { damagePopup.Initialize(transform); damagePopup.gameObject.SetActive(true); }
    }
    public void ShowDamage(float damage) { if (damagePopup != null) damagePopup.ShowDamage(damage); }
    protected abstract Vector3 GetMoveDirection();
    protected virtual void FixedUpdate() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) { rb.linearVelocity = Vector3.zero; return; }
        if (player == null) return;
        Vector3 dir = GetMoveDirection();
        rb.linearVelocity = new Vector3(dir.x, rb.linearVelocity.y, dir.z) * moveSpeed;
        TryAttack();
    }
    protected virtual void TryAttack() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        if (player == null) return;
        if (Time.time < nextAttack) return;
        if (Vector3.Distance(transform.position, player.position) <= attackRange) {
            nextAttack = Time.time + attackCooldown;
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySoundEffect("EnemyAttack");
            if (playerHealth != null) playerHealth.TakeDamage(damage * difficultyMultiplier);
        }
    }
    public void SetDifficultyMultiplier(float mult) { difficultyMultiplier = Mathf.Max(0.5f, mult); }
    public void SetPool(EnemyPool enemyPool) => pool = enemyPool;
    public void SetSpawner(EnemySpawner enemySpawner) => spawner = enemySpawner;
    protected virtual void OnDeath() {
        if (healthDisplay != null && EnemyHealthPool.Instance != null) EnemyHealthPool.Instance.Release(healthDisplay);
        if (damagePopup != null && DamagePopupPool.Instance != null) DamagePopupPool.Instance.Release(damagePopup);
        healthDisplay = null;
        damagePopup = null;
        if (ParticleEffectManager.Instance != null) ParticleEffectManager.Instance.PlayDeathEffect(transform.position);
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySoundEffect("EnemyDeath");
        if (spawner != null) spawner.OnEnemyDied(Type);
        var xpSystem = FindFirstObjectByType<ExperienceSystem>();
        if (xpSystem != null && GameManager.Instance != null) {
            int xp = GameManager.Instance.GetEnemyXpReward(Type, spawnedAtWave);
            xpSystem.AddXP(xp);
        }
        if (pool != null) pool.Release(this);
        else Destroy(gameObject);
    }
    protected virtual void OnDisable() {
        if (healthDisplay != null) healthDisplay.gameObject.SetActive(false);
        if (damagePopup != null) { damagePopup.ClearAllItems(); damagePopup.gameObject.SetActive(false); }
    }
    public void SetSpawnWave(int wave) => spawnedAtWave = wave;
}
