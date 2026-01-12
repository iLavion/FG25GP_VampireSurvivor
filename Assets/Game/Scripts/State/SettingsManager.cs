using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class GameSettings {
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public int displayMode = 1; // 0="Windowed" | 1="Windowed Fullscreen" | 2="Fullscreen"
    public bool vSync = true;
    public float volume = 1.0f;
}
public class SettingsManager : MonoBehaviour {
    public static SettingsManager Instance { get; private set; }
    public GameSettings CurrentSettings { get; private set; }
    private GameSettings workingSettings;
    private string settingsFilePath;
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplySettings();
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        settingsFilePath = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings();
        ApplySettings();
    }
    private void Start() => ApplyVolume();
    public void LoadSettings() {
        if (File.Exists(settingsFilePath)) {
            try {
                string json = File.ReadAllText(settingsFilePath);
                CurrentSettings = JsonUtility.FromJson<GameSettings>(json);
                Debug.Log($"Settings loaded from: {settingsFilePath}");
            } catch (Exception e) {
                Debug.LogError($"Failed to load settings: {e.Message}");
                CurrentSettings = new GameSettings();
                SaveSettingsToFile();
            }
        } else { CurrentSettings = new GameSettings(); SaveSettingsToFile(); }
    }
    public void ResetToDefaultsAndSave() {
        workingSettings = null;
        CurrentSettings = new GameSettings();
        ApplySettings();
        SaveSettingsToFile();
    }
    private static GameSettings Clone(GameSettings src) => JsonUtility.FromJson<GameSettings>(JsonUtility.ToJson(src));
    private void EnsureWorking() => workingSettings ??= Clone(CurrentSettings);
    public void BeginEditing() => workingSettings = Clone(CurrentSettings);
    public void DiscardPending() { workingSettings = null; }
    public void CommitPending() {
        if (workingSettings == null) return;
        CurrentSettings = Clone(workingSettings);
        workingSettings = null;
        ApplySettings();
        SaveSettingsToFile();
    }
    public GameSettings GetActiveSettingsSnapshot() => workingSettings ?? CurrentSettings;
    public void SetPendingDisplayMode(int mode) { EnsureWorking(); workingSettings.displayMode = mode; }
    public void SetPendingVSync(bool enabled) { EnsureWorking(); workingSettings.vSync = enabled; }
    public void SetPendingVolume(float vol) { EnsureWorking(); workingSettings.volume = vol; }
    private void SaveSettingsToFile() {
        try {
            string json = JsonUtility.ToJson(CurrentSettings, true);
            File.WriteAllText(settingsFilePath, json);
            Debug.Log($"Settings saved to {settingsFilePath}");
        } catch (Exception e) {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }
    public void ApplySettings() {
        ApplyResolution();
        ApplyDisplayMode();
        ApplyVSync();
        ApplyVolume();
    }
    private void ApplyResolution() => Screen.SetResolution(CurrentSettings.resolutionWidth, CurrentSettings.resolutionHeight, Screen.fullScreenMode);
    private void ApplyDisplayMode() {
        FullScreenMode mode = CurrentSettings.displayMode switch {
            0 => FullScreenMode.Windowed,
            1 => FullScreenMode.FullScreenWindow,
            2 => FullScreenMode.ExclusiveFullScreen,
            _ => FullScreenMode.FullScreenWindow
        };
        Screen.fullScreenMode = mode;
    }
    private void ApplyVSync() => QualitySettings.vSyncCount = CurrentSettings.vSync ? 1 : 0;
    private void ApplyVolume() { if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(CurrentSettings.volume);}
    public void UpdateResolution(int width, int height) {
        CurrentSettings.resolutionWidth = width;
        CurrentSettings.resolutionHeight = height;
        ApplyResolution();
    }
    public void UpdateDisplayMode(int mode) { CurrentSettings.displayMode = mode; ApplyDisplayMode(); }
    public void UpdateVSync(bool enabled) { CurrentSettings.vSync = enabled; ApplyVSync(); }
    public void UpdateVolume(float vol) {
        CurrentSettings.volume = vol;
        if (AudioManager.Instance != null) AudioManager.Instance.SetMasterVolume(vol);
    }
}
