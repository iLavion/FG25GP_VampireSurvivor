using UnityEngine;
using TMPro;

public class GameHUD : MonoBehaviour {
    [Header("UI Text Elements")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI staminaText;
    private Health playerHealth;
    private Stamina playerStamina;
    private ExperienceSystem playerXP;
    private void Start() {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = FindFirstObjectByType<PlayerController>()?.gameObject;
        if (player != null) { playerHealth = player.GetComponent<Health>(); playerStamina = player.GetComponent<Stamina>(); playerXP = player.GetComponent<ExperienceSystem>(); }
        if (playerHealth != null) { playerHealth.onHealthChanged.AddListener(OnHealthChanged); OnHealthChanged(playerHealth.Current, playerHealth.Max); }
        if (playerStamina != null) { playerStamina.onStaminaChanged.AddListener(OnStaminaChanged); OnStaminaChanged(playerStamina.Current, playerStamina.Max); }
        else { if (staminaText != null) staminaText.text = "SP. N/A"; }
        if (playerXP != null) { playerXP.onXPChanged.AddListener(OnXPChanged); playerXP.onLevelUp.AddListener(OnLevelUp); UpdateXPDisplay(); }
        if (GameManager.Instance != null) { GameManager.Instance.onWaveChanged.AddListener(OnWaveChanged); OnWaveChanged(GameManager.Instance.Wave); }
    }
    private void OnDestroy() {
        if (playerHealth != null) playerHealth.onHealthChanged.RemoveListener(OnHealthChanged);
        if (playerStamina != null) playerStamina.onStaminaChanged.RemoveListener(OnStaminaChanged);
        if (playerXP != null) { playerXP.onXPChanged.RemoveListener(OnXPChanged); playerXP.onLevelUp.RemoveListener(OnLevelUp); }
        if (GameManager.Instance != null) GameManager.Instance.onWaveChanged.RemoveListener(OnWaveChanged);
    }
    private void OnWaveChanged(int wave) { if (waveText != null) waveText.text = $"Wave {wave}"; }
    private void OnHealthChanged(float current, float max) { if (healthText != null) healthText.text = $"HP. {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}"; }
    private void OnStaminaChanged(float current, float max) { if (staminaText != null) staminaText.text = $"SP. {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}"; }
    private void OnXPChanged(int level, int currentXP, int xpToNext) => UpdateXPDisplay();
    private void OnLevelUp(int newLevel) => UpdateXPDisplay();
    private void UpdateXPDisplay() { if (playerXP == null) return; if (levelText != null) levelText.text = $"Lv. {playerXP.Level} ({playerXP.CurrentXP} / {playerXP.XPToNext})"; }
}
