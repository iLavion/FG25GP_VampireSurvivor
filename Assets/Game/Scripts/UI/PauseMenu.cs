using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour {
    [SerializeField] private GameObject panel;
    [SerializeField] private Leaderboard leaderboard;
    private void Start() {
        if (panel != null) panel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.onStateChanged.AddListener(OnStateChanged);
    }
    private void OnDestroy() {
        if (GameManager.Instance != null) GameManager.Instance.onStateChanged.RemoveListener(OnStateChanged);
    }
    private void Update() { var keyboard = Keyboard.current; if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) TogglePause(); }
    private void OnStateChanged(GameState state) {
        if (panel == null) return;
        bool show = state == GameState.Paused;
        panel.SetActive(show);
        if (show && leaderboard != null) leaderboard.Refresh();
    }
    private void TogglePause() {
        if (GameManager.Instance == null) return;
        var state = GameManager.Instance.Machine.State;
        if (state == GameState.Paused) GameManager.Instance.ResumeGame();
        else if (state == GameState.Playing) GameManager.Instance.PauseGame();
        else if (state == GameState.Settings) {
            if (SettingsMenu.Instance != null) SettingsMenu.Instance.CloseSettings();
        }
    }
    public void Pause() { if (GameManager.Instance != null) GameManager.Instance.PauseGame(); }
    public void Resume() { if (GameManager.Instance != null) GameManager.Instance.ResumeGame(); }
    public void OpenSettings() {
        if (SettingsMenu.Instance != null) {
            SettingsMenu.Instance.Open();
            if (panel != null) panel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.OpenSettings();
        }
    }
    public void ReturnToMainMenu() {
        if (GameManager.Instance != null) GameManager.Instance.ReturnToMainMenu();
        if (TransitionManager.Instance != null) TransitionManager.Instance.FadeOutThen(() => SceneManager.LoadSceneAsync(0));
        else SceneManager.LoadSceneAsync(0);
    }
    public void QuitGame() => Application.Quit();
}
