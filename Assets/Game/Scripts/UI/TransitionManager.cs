using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour {
    public static TransitionManager Instance { get; private set; }
    [SerializeField, Range(0.1f, 2f)] private float transitionDuration = 0.5f;
    private Image fadeImage;
    private Canvas fadeCanvas;
    private Coroutine fadeCoroutine;
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        CreateFadeCanvas();
    }
    private void CreateFadeCanvas() {
        GameObject canvasObj = new("FadeCanvas");
        canvasObj.transform.SetParent(transform);
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 9999;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        GameObject fadeObj = new("FadeImage");
        fadeObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = fadeObj.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;
        RectTransform rect = fadeObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
    private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => FadeIn();
    public void FadeOut(System.Action onComplete = null) => FadeToAlpha(1f, onComplete);
    public void FadeIn(System.Action onComplete = null) => FadeToAlpha(0f, onComplete);
    public void FadeOutThen(System.Action action) { FadeOut(() => { action?.Invoke(); }); }
    private void FadeToAlpha(float targetAlpha, System.Action onComplete = null) {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCoroutine(targetAlpha, onComplete));
    }
    private IEnumerator FadeCoroutine(float targetAlpha, System.Action onComplete = null) {
        if (fadeImage == null) yield break;
        float elapsedTime = 0f;
        Color startColor = fadeImage.color;
        Color endColor = new(startColor.r, startColor.g, startColor.b, targetAlpha);
        while (elapsedTime < transitionDuration) {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / transitionDuration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        fadeImage.color = endColor;
        onComplete?.Invoke();
    }
}
