using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
    public static CameraShake Instance { get; private set; }
    private Coroutine shakeCoroutine;
    private Vector3 originalPosition;
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    public void Shake(float duration = 0.2f, float magnitude = 0.3f) {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    private IEnumerator ShakeCoroutine(float duration, float magnitude) {
        originalPosition = transform.localPosition;
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            elapsedTime += Time.unscaledDeltaTime;
            float randomX = Random.Range(-1f, 1f) * magnitude;
            float randomY = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(randomX, randomY, 0f);
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
