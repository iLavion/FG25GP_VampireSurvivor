using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeMenu : MonoBehaviour {
    [SerializeField] private GameObject upgradeChoicePrefab;
    [SerializeField] private Transform choicesContainer;
    private UpgradeManager upgradeManager;
    private void Start() {
        if (upgradeManager == null) upgradeManager = FindFirstObjectByType<UpgradeManager>();
        if (upgradeManager != null) { upgradeManager.onShowChoices.AddListener(DisplayUpgrades); upgradeManager.onHideChoices.AddListener(HideUpgrades); }
        gameObject.SetActive(false);
    }
    private void OnDestroy() {
        if (upgradeManager != null) {
            upgradeManager.onShowChoices.RemoveListener(DisplayUpgrades);
            upgradeManager.onHideChoices.RemoveListener(HideUpgrades);
        }
    }
    private void DisplayUpgrades(List<ScriptableUpgrade> upgrades) {
        ClearChoices();
        gameObject.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.OpenUpgradeMenu();
        foreach (var upgrade in upgrades) {
            if (upgrade == null || upgradeChoicePrefab == null) continue;
            var choice = Instantiate(upgradeChoicePrefab, choicesContainer);
            var button = choice.GetComponent<Button>();
            if (button != null) button.onClick.AddListener(() => SelectUpgrade(upgrade));
            var displayName = choice.GetComponentInChildren<TextMeshProUGUI>(includeInactive: true);
            if (displayName != null) displayName.text = upgrade.DisplayName;
            var description = choice.transform.Find("Description");
            if (description != null) { if (description.TryGetComponent<TextMeshProUGUI>(out var descText)) descText.text = upgrade.Description; }
        }
    }
    private void ClearChoices() { foreach (Transform child in choicesContainer) Destroy(child.gameObject); }
    private void SelectUpgrade(ScriptableUpgrade upgrade) { if (upgradeManager != null) upgradeManager.ChooseUpgrade(upgrade); }
    private void HideUpgrades() { ClearChoices(); gameObject.SetActive(false); if (GameManager.Instance != null) GameManager.Instance.CloseUpgradeMenu(); }
}
