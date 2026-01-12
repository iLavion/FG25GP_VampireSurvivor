using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum UpgradeRarity { Common, Uncommon, Rare, Epic, Legendary }

[System.Serializable]
public class EnemyConfig {
    public EnemyType type;
    public EnemyBase prefab;
    public int introducedAtWave = 1;
    public float difficultyMultiplier = 1.2f;
    public int poolSize = 20;
    [Header("XP")]
    public int baseXpReward = 10;
    public float xpGrowthPerWave = 1.15f;
}
[System.Serializable]
public class UpgradeConfig {
    public ScriptableUpgrade upgrade;
    public int unlockedAtLevel = 1;
}
[System.Serializable]
public class RarityWeight {
    public UpgradeRarity rarity;
    [Range(1, 100)] public int weight = 50;
}
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public StateMachine<GameState> Machine { get; private set; } = new StateMachine<GameState>();
    private Canvas gameHUDCanvas;
    [Header("Wave & Difficulty")]
    [SerializeField] private int startingWave = 1;
    [SerializeField] private float waveIncrement = 1.2f;
    [Header("Enemies")]
    [SerializeField] private List<EnemyConfig> enemies = new();
    [Header("Prefabs")]
    [SerializeField] private DamagePopup damagePopupPrefab;
    [SerializeField] private EnemyHealth enemyHealthPrefab;
    [Header("Upgrades")]
    [SerializeField] private List<UpgradeConfig> upgrades = new();
    [SerializeField]
    private List<RarityWeight> rarityWeights = new()
    {
        new() { rarity = UpgradeRarity.Common, weight = 60 },
        new() { rarity = UpgradeRarity.Uncommon, weight = 25 },
        new() { rarity = UpgradeRarity.Rare, weight = 10 },
        new() { rarity = UpgradeRarity.Epic, weight = 4 },
        new() { rarity = UpgradeRarity.Legendary, weight = 1 }
    };
    [Header("XP Scaling")]
    [SerializeField] private int baseXpToLevel = 10;
    [SerializeField] private float xpLevelGrowth = 1.5f;
    private int currentWave;
    private float difficultyMultiplier;
    private GameState previousState;
    public UnityEvent<int> onWaveChanged;
    public UnityEvent<GameState> onStateChanged;
    public IReadOnlyList<EnemyConfig> Enemies => enemies;
    public IReadOnlyList<UpgradeConfig> Upgrades => upgrades;
    public int BaseXpToLevel => baseXpToLevel;
    public float XpLevelGrowth => xpLevelGrowth;
    public int Wave => currentWave;
    public float DifficultyMultiplier => difficultyMultiplier;
    public DamagePopup DamagePopupPrefab => damagePopupPrefab;
    public EnemyHealth EnemyHealthPrefab => enemyHealthPrefab;
    public Canvas GameHUDCanvas => gameHUDCanvas;
    public bool IsPaused => 
        Machine.State == GameState.Paused
        || Machine.State == GameState.Upgrade
        || Machine.State == GameState.Settings
        || Machine.State == GameState.GameOver
        || Machine.State == GameState.MainMenu;
    public bool IsPlaying => Machine.State == GameState.Playing;
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        var hud = FindFirstObjectByType<GameHUD>();
        if (hud != null) {
            var hudCanvas = hud.GetComponentInParent<Canvas>();
            gameHUDCanvas = hudCanvas != null ? hudCanvas : FindFirstObjectByType<Canvas>();
        } else gameHUDCanvas = FindFirstObjectByType<Canvas>();
        Machine.OnStateChanged += HandleStateChanged;
        SetWave(startingWave);
        Machine.SetState(GameState.Playing);
    }
    private void OnDestroy() => Machine.OnStateChanged -= HandleStateChanged;
    private void HandleStateChanged(GameState newState) {
        onStateChanged?.Invoke(newState);
        if (newState == GameState.GameOver && AudioManager.Instance != null) AudioManager.Instance.StopMusic();
    }
    public void StartGame() { SetWave(startingWave); Machine.SetState(GameState.Playing); }
    public void PauseGame() {
        if (Machine.State == GameState.Playing) { previousState = Machine.State; Machine.SetState(GameState.Paused); }
    }
    public void ResumeGame() {
        if (Machine.State == GameState.Paused || Machine.State == GameState.Settings) Machine.SetState(GameState.Playing);
    }
    public void OpenUpgradeMenu() {
        if (Machine.State == GameState.Playing) { previousState = Machine.State; Machine.SetState(GameState.Upgrade); }
    }
    public void CloseUpgradeMenu() { if (Machine.State == GameState.Upgrade) Machine.SetState(GameState.Playing); }
    public void OpenSettings() { previousState = Machine.State;  Machine.SetState(GameState.Settings); }
    public void CloseSettings() => Machine.SetState(previousState);
    public void TriggerGameOver() => Machine.SetState(GameState.GameOver);
    public void ReturnToMainMenu() => Machine.SetState(GameState.MainMenu);
    private void SetWave(int wave) {
        currentWave = Mathf.Max(1, wave);
        difficultyMultiplier = Mathf.Pow(waveIncrement, currentWave - 1);
        onWaveChanged?.Invoke(currentWave);
    }
    public void NextWave() => SetWave(currentWave + 1);
    public EnemyConfig GetEnemyConfig(EnemyType type) {
        foreach (var cfg in enemies) if (cfg.type == type) return cfg;
        return null;
    }
    public List<EnemyType> GetAvailableEnemies(int wave) {
        var available = new List<EnemyType>();
        foreach (var cfg in enemies) if (wave >= cfg.introducedAtWave) available.Add(cfg.type);
        return available;
    }
    public float GetEnemyDifficultyMultiplier(EnemyType type, int wave) {
        var cfg = GetEnemyConfig(type);
        if (cfg != null) {
            int wavesSinceIntroduced = Mathf.Max(0, wave - cfg.introducedAtWave);
            return Mathf.Pow(cfg.difficultyMultiplier, wavesSinceIntroduced);
        }
        return 1f;
    }
    public int GetEnemyXpReward(EnemyType type, int wave) {
        var cfg = GetEnemyConfig(type);
        if (cfg != null) {
            int wavesSinceIntroduced = Mathf.Max(0, wave - cfg.introducedAtWave);
            return Mathf.RoundToInt(cfg.baseXpReward * Mathf.Pow(cfg.xpGrowthPerWave, wavesSinceIntroduced));
        }
        return 10;
    }
    public List<ScriptableUpgrade> PickRandomUpgrades(int playerLevel, int count, List<ScriptableUpgrade> exclude = null) {
        var pool = new List<UpgradeConfig>();
        foreach (var cfg in upgrades) {
            if (cfg.upgrade == null) continue;
            if (playerLevel < cfg.unlockedAtLevel) continue;
            if (exclude != null && exclude.Contains(cfg.upgrade)) continue;
            pool.Add(cfg);
        }
        var picks = new List<ScriptableUpgrade>();
        while (picks.Count < count && pool.Count > 0) {
            UpgradeRarity rolledRarity = RollRarity();
            var rarityPool = pool.FindAll(c => c.upgrade.Rarity == rolledRarity);
            if (rarityPool.Count == 0) rarityPool = pool;
            int totalWeight = 0;
            foreach (var c in rarityPool) totalWeight += c.upgrade.Weight;
            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;
            UpgradeConfig picked = null;
            foreach (var c in rarityPool) {
                cumulative += c.upgrade.Weight;
                if (roll < cumulative) { picked = c; break; }
            }
            if (picked != null) { picks.Add(picked.upgrade); pool.Remove(picked); }
        }
        return picks;
    }
    private UpgradeRarity RollRarity() {
        int totalWeight = 0;
        foreach (var rw in rarityWeights) totalWeight += rw.weight;
        int roll = Random.Range(0, totalWeight);
        int cumulative = 0;
        foreach (var rw in rarityWeights) {
            cumulative += rw.weight;
            if (roll < cumulative) return rw.rarity;
        }
        return UpgradeRarity.Common;
    }
}
