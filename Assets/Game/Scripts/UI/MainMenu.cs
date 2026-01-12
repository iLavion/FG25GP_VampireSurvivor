using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private Leaderboard leaderboard;
    private void Start() {
        if (panel != null) panel.SetActive(true);
        if (SettingsMenu.Instance != null) SettingsMenu.Instance.onSettingsClosed.AddListener(OnSettingsClosed);
        if (GameManager.Instance != null) {
            GameManager.Instance.ReturnToMainMenu();
            GameManager.Instance.onStateChanged.AddListener(OnStateChanged);
        }
        RefreshLeaderboard();
    }
    private void OnDestroy() {
        if (SettingsMenu.Instance != null) SettingsMenu.Instance.onSettingsClosed.RemoveListener(OnSettingsClosed);
        if (GameManager.Instance != null) GameManager.Instance.onStateChanged.RemoveListener(OnStateChanged);
    }
    private void OnStateChanged(GameState state) {
        if (state == GameState.MainMenu && panel != null) panel.SetActive(true);
        else if (state == GameState.Settings && panel != null) panel.SetActive(false);
    }
    private void OnSettingsClosed() { if (panel != null) panel.SetActive(true); }
    private void RefreshLeaderboard() { if (leaderboard != null) leaderboard.Refresh(); }
    public void PlayGame() {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameMusic();
        if (TransitionManager.Instance != null) TransitionManager.Instance.FadeOutThen(() => SceneManager.LoadSceneAsync(1));
        else SceneManager.LoadSceneAsync(1);
    }
    public void OpenSettings() {
        if (SettingsMenu.Instance != null) {
            if (panel != null) panel.SetActive(false);
            SettingsMenu.Instance.Open();
            if (GameManager.Instance != null) GameManager.Instance.OpenSettings();
        }
    }
    public void ExitGame() => Application.Quit();
}
