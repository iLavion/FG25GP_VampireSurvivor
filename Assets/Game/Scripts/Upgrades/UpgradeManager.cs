using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour {
    [SerializeField] private int choicesCount = 3;
    public UnityEvent<List<ScriptableUpgrade>> onShowChoices;
    public UnityEvent onHideChoices;
    private UpgradeContext ctx;
    private ExperienceSystem xpSystem;
    private void Start() {
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) {
            ctx = new UpgradeContext {
                Player = player,
                Combat = player.GetComponent<PlayerCombat>(),
                Health = player.GetComponent<Health>(),
                XP = player.GetComponent<ExperienceSystem>(),
                Upgrades = player.GetComponent<PlayerUpgrades>()
            };
            xpSystem = ctx.XP;
            if (xpSystem != null) xpSystem.onLevelUp.AddListener(OnLevelUp);
        }
    }
    private void OnDestroy() { if (xpSystem != null) xpSystem.onLevelUp.RemoveListener(OnLevelUp); }
    private void OnLevelUp(int level) {
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySoundEffect("LevelUp");
        ShowChoices(level);
    }
    public void ShowChoices(int playerLevel) {
        if (GameManager.Instance == null) return;
        var picks = GameManager.Instance.PickRandomUpgrades(playerLevel, choicesCount);
        if (picks.Count > 0) onShowChoices?.Invoke(picks);
    }
    public void ChooseUpgrade(ScriptableUpgrade upgrade) {
        if (upgrade != null && ctx != null) upgrade.Apply(ctx);
        onHideChoices?.Invoke();
    }
}
