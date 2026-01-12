using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndMenu : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text statsText;
    [SerializeField] private Leaderboard leaderboard;
    private ExperienceSystem xpSystem;
    private int cachedScore;
    private int cachedWave;
    private int cachedLevel;
    private bool shown;
    private void Start() {
        if (panel == null) panel = transform.GetChild(0).gameObject;
        if (panel != null) panel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.onStateChanged.AddListener(OnStateChanged);
    }
    private void OnDestroy() { if (GameManager.Instance != null) GameManager.Instance.onStateChanged.RemoveListener(OnStateChanged); }
    private void OnStateChanged(GameState state) {
        if (!shown) return;
        if (panel == null) return;
        if (state == GameState.GameOver) panel.SetActive(true);
        else if (state == GameState.Settings) panel.SetActive(false);
    }
    public void Show() {
        if (shown) return;
        shown = true;
        if (panel != null) panel.SetActive(true);
        if (GameManager.Instance != null) GameManager.Instance.TriggerGameOver();
        PopulateStats();
    }
    private void PopulateStats() {
        if (xpSystem == null) {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) xpSystem = player.GetComponent<ExperienceSystem>();
        }
        cachedWave = GameManager.Instance != null ? GameManager.Instance.Wave : 1;
        cachedLevel = xpSystem != null ? xpSystem.Level : 1;
        int totalXP = xpSystem != null ? xpSystem.TotalXP : 0;
        cachedScore = (cachedWave - 1) * 1000 + totalXP;
        if (statsText != null) statsText.text = $"Score: {cachedScore}";
    }
    public void SubmitScore() {
        string username = usernameInput != null && !string.IsNullOrWhiteSpace(usernameInput.text) ? usernameInput.text : "Player";
        var list = HighscoreStorage.Load();
        var existingEntry = list.entries.Find(e => e.username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (existingEntry != null) {
            existingEntry.score = cachedScore;
            existingEntry.wave = cachedWave;
            existingEntry.level = cachedLevel;
            existingEntry.timestamp = DateTime.UtcNow.ToString("o");
        } else {
            var entry = new HighscoreEntry {
                username = username,
                score = cachedScore,
                wave = cachedWave,
                level = cachedLevel,
                timestamp = DateTime.UtcNow.ToString("o")
            };
            list.entries.Add(entry);
        }
        list.entries.Sort((a, b) => b.score.CompareTo(a.score));
        HighscoreStorage.Save(list);
        if (leaderboard != null) leaderboard.Refresh();
    }
    public void RestartGame() {
        if (GameManager.Instance != null) GameManager.Instance.StartGame();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
    public void ReturnToMainMenu() {
        if (GameManager.Instance != null) GameManager.Instance.ReturnToMainMenu();
        if (TransitionManager.Instance != null) TransitionManager.Instance.FadeOutThen(() => SceneManager.LoadSceneAsync(0));
        else SceneManager.LoadSceneAsync(0);
    }
    public void OpenSettings() {
        if (SettingsMenu.Instance != null) {
            SettingsMenu.Instance.Open();
            if (panel != null) panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.OpenSettings();
        }
    }
}
