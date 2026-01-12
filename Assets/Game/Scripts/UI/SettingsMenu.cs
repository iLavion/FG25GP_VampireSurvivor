using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour {
    public static SettingsMenu Instance { get; private set; }
    [SerializeField] private GameObject panel;
    [Header("UI Controls")]
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider volumeSlider;
    public UnityEvent onSettingsClosed;
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start() { if (panel != null) panel.SetActive(false); }
    public void Open() { if (panel != null) panel.SetActive(true); LoadCurrentSettings(); }
    private void LoadCurrentSettings() {
        if (SettingsManager.Instance == null) return;
        var settings = SettingsManager.Instance.CurrentSettings;
        if (settings == null) return;
        if (displayModeDropdown != null) displayModeDropdown.SetValueWithoutNotify(settings.displayMode);
        if (vsyncToggle != null) vsyncToggle.SetIsOnWithoutNotify(settings.vSync);
        if (volumeSlider != null) volumeSlider.SetValueWithoutNotify(settings.volume);
    }
    public GameSettings GetSettingsSnapshot() => SettingsManager.Instance != null ? SettingsManager.Instance.CurrentSettings : null;
    public void SetDisplayMode(int modeIndex) { if (SettingsManager.Instance != null) SettingsManager.Instance.SetPendingDisplayMode(modeIndex); }
    public void SetVSync(bool enabled) { if (SettingsManager.Instance != null) SettingsManager.Instance.SetPendingVSync(enabled); }
    public void SetVolume(float volume) { if (SettingsManager.Instance != null) SettingsManager.Instance.SetPendingVolume(volume); }
    public void SaveSettings() { if (SettingsManager.Instance != null) SettingsManager.Instance.CommitPending(); }
    public void CloseSettings() {
        if (SettingsManager.Instance != null) SettingsManager.Instance.DiscardPending();
        if (panel != null) panel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.CloseSettings();
        onSettingsClosed?.Invoke();
    }
}
