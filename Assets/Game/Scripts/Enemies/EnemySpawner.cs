using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour {
    [SerializeField] private EnemyFactory factory;
    [SerializeField] private float initialDelay = 0.5f;
    [SerializeField] private float spawnInterval = 0.3f;
    [SerializeField] private float minSpawnDistance = 12f;
    [SerializeField] private float maxSpawnDistance = 25f;
    private const float WORLD_UP_Y = 0f;
    private const int MAX_SPAWN_ATTEMPTS = 10;
    private Camera mainCamera;
    private readonly Dictionary<EnemyType, int> enemyTypeWaveIntroduced = new()
    {
        { EnemyType.Chaser, 1 },
        { EnemyType.Orbiter, 3 },
        { EnemyType.Boss, 1 }
    };
    private float timer;
    private readonly Dictionary<EnemyType, int> enemiesSpawnedByType = new();
    private readonly Dictionary<EnemyType, int> enemiesToSpawnByType = new();
    private int activeEnemyCount;
    private int normalEnemyCount;
    private Vector3 cachedPlayerPos;
    private bool normalEnemiesComplete;
    private bool bossPhase;
    private void Start() {
        timer = -initialDelay;
        if (factory == null) factory = FindFirstObjectByType<EnemyFactory>();
        if (GameManager.Instance != null) { GameManager.Instance.onWaveChanged.AddListener(OnWaveChanged); StartWave(GameManager.Instance.Wave); }
    }
    private void OnDestroy() { if (GameManager.Instance != null) GameManager.Instance.onWaveChanged.RemoveListener(OnWaveChanged); }
    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        timer += Time.deltaTime;
        if (!normalEnemiesComplete && !bossPhase && timer >= spawnInterval) { timer = 0f; TrySpawnNextEnemy(); }
        if (!normalEnemiesComplete && AreAllNormalEnemiesSpawned() && normalEnemyCount <= 0) CompleteNormalEnemies();
        if (bossPhase && timer >= spawnInterval) { timer = 0f; TrySpawnBoss(); }
        if (bossPhase && AreAllBossesSpawned() && activeEnemyCount <= 0) CompleteWave();
    }
    private void StartWave(int wave) {
        normalEnemiesComplete = false;
        bossPhase = false;
        activeEnemyCount = 0;
        normalEnemyCount = 0;
        enemiesSpawnedByType.Clear();
        enemiesToSpawnByType.Clear();
        var available = GameManager.Instance.GetAvailableEnemies(wave);
        foreach (var enemyType in available) {
            if (enemyType == EnemyType.Boss) continue;
            if (enemyTypeWaveIntroduced.TryGetValue(enemyType, out int introWave)) {
                int fibonacciIndex = wave - introWave + 1;
                int spawnCount = GetFibonacci(fibonacciIndex, false);
                enemiesToSpawnByType[enemyType] = spawnCount;
                enemiesSpawnedByType[enemyType] = 0;
            }
        }
        cachedPlayerPos = FindPlayerPos();
    }
    private int GetFibonacci(int n, bool isBoss) {
        if (isBoss) {
            if (n <= 0) return 1; if (n == 1) return 1; if (n == 2) return 2; if (n == 3) return 3;
            int a = 3, b = 5;
            for (int i = 4; i < n; i++) { int temp = a + b; a = b; b = temp; }
            return n == 4 ? 5 : b;
        } else {
            if (n <= 0) return 5; if (n == 1) return 5; if (n == 2) return 8;
            int a = 5, b = 8;
            for (int i = 3; i <= n; i++) { int temp = a + b; a = b; b = temp; }
            return b;
        }
    }
    private bool TrySpawnNextEnemy() {
        if (factory == null || GameManager.Instance == null) return false;
        foreach (var kvp in enemiesToSpawnByType) {
            EnemyType type = kvp.Key;
            int toSpawn = kvp.Value;
            int spawned = enemiesSpawnedByType.ContainsKey(type) ? enemiesSpawnedByType[type] : 0;
            if (spawned < toSpawn) {
                int wave = GameManager.Instance.Wave;
                float diff = GameManager.Instance.GetEnemyDifficultyMultiplier(type, wave);
                Vector3 spawnPos = GetSpawnPosition();
                var enemy = factory.Create(type, spawnPos, diff);
                if (enemy != null) {
                    enemy.SetSpawner(this);
                    enemy.SetSpawnWave(wave);
                    enemiesSpawnedByType[type] = spawned + 1;
                    activeEnemyCount++;
                    normalEnemyCount++;
                    return true;
                }
            }
        }
        return false;
    }
    private bool AreAllNormalEnemiesSpawned() {
        foreach (var kvp in enemiesToSpawnByType) {
            int spawned = enemiesSpawnedByType.ContainsKey(kvp.Key) ? enemiesSpawnedByType[kvp.Key] : 0;
            if (spawned < kvp.Value) return false;
        }
        return true;
    }
    private bool AreAllBossesSpawned() {
        if (!enemiesToSpawnByType.ContainsKey(EnemyType.Boss)) return true;
        int spawned = enemiesSpawnedByType.ContainsKey(EnemyType.Boss) ? enemiesSpawnedByType[EnemyType.Boss] : 0;
        return spawned >= enemiesToSpawnByType[EnemyType.Boss];
    }
    private void CompleteNormalEnemies() { normalEnemiesComplete = true; StartBossPhase(); }
    private void StartBossPhase() {
        int wave = GameManager.Instance != null ? GameManager.Instance.Wave : 1;
        if (enemyTypeWaveIntroduced.TryGetValue(EnemyType.Boss, out int bossIntroWave)) {
            if (wave >= bossIntroWave) {
                bossPhase = true;
                int fibonacciIndex = wave - bossIntroWave + 1;
                int bossCount = GetFibonacci(fibonacciIndex, true);
                enemiesToSpawnByType[EnemyType.Boss] = bossCount;
                enemiesSpawnedByType[EnemyType.Boss] = 0;
                return;
            }
        }
        if (GameManager.Instance != null) GameManager.Instance.NextWave();
    }
    private void CompleteWave() { if (GameManager.Instance != null) GameManager.Instance.NextWave(); }
    private void OnWaveChanged(int wave) => StartWave(wave);
    private bool TrySpawnBoss() {
        if (factory == null || GameManager.Instance == null) return false;
        if (!enemiesToSpawnByType.ContainsKey(EnemyType.Boss)) return false;
        int spawned = enemiesSpawnedByType.ContainsKey(EnemyType.Boss) ? enemiesSpawnedByType[EnemyType.Boss] : 0;
        int toSpawn = enemiesToSpawnByType[EnemyType.Boss];
        if (spawned < toSpawn) {
            int wave = GameManager.Instance.Wave;
            float diff = GameManager.Instance.GetEnemyDifficultyMultiplier(EnemyType.Boss, wave);
            Vector3 spawnPos = GetSpawnPosition();
            var boss = factory.Create(EnemyType.Boss, spawnPos, diff);
            if (boss != null) {
                boss.SetSpawner(this);
                boss.SetSpawnWave(wave);
                enemiesSpawnedByType[EnemyType.Boss] = spawned + 1;
                activeEnemyCount++;
                return true;
            }
        }
        return false;
    }
    public void OnEnemyDied(EnemyType type) {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        if (type != EnemyType.Boss) normalEnemyCount = Mathf.Max(0, normalEnemyCount - 1);
    }
    private Vector3 FindPlayerPos() {
        var p = FindFirstObjectByType<PlayerController>();
        return p != null ? p.transform.position : Vector3.zero;
    }
    private Vector3 GetSpawnPosition() {
        if (mainCamera == null) mainCamera = Camera.main;
        Vector3 playerPos = cachedPlayerPos;
        if (playerPos == Vector3.zero) playerPos = FindPlayerPos();
        for (int i = 0; i < MAX_SPAWN_ATTEMPTS; i++) {
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 spawnPos = new Vector3(randomDir.x * distance, WORLD_UP_Y, randomDir.y * distance) + playerPos;
            if (!IsPositionVisible(spawnPos)) return spawnPos;
        }
        Vector2 fallbackDir = Random.insideUnitCircle.normalized;
        return new Vector3(fallbackDir.x * maxSpawnDistance, WORLD_UP_Y, fallbackDir.y * maxSpawnDistance) + playerPos;
    }
    private bool IsPositionVisible(Vector3 worldPos) {
        if (mainCamera == null) return false;
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(worldPos);
        const float margin = 0.05f;
        return viewportPos.z > 0 && viewportPos.x > -margin && viewportPos.x < 1f + margin && viewportPos.y > -margin && viewportPos.y < 1f + margin;
    }
}
