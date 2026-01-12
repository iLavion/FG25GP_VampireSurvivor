using UnityEngine;
using TMPro;
using System.Collections;

public class DamagePopupItem : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float lifetime = 1f;
    private System.Action onComplete;
    public void Initialize(float damage, System.Action onItemComplete) {
        gameObject.SetActive(true);
        if (damageText != null) {
            damageText.text = "-" + Mathf.RoundToInt(damage).ToString();
            Color c = damageText.color;
            c.a = 1f;
            damageText.color = c;
        }
        onComplete = onItemComplete;
        StartCoroutine(FadeOut());
    }
    private IEnumerator FadeOut() {
        float elapsed = 0f;
        while (elapsed < lifetime) {
            elapsed += Time.unscaledDeltaTime;
            if (damageText != null) { Color c = damageText.color; c.a = 1f - (elapsed / lifetime); damageText.color = c; }
            yield return null;
        }
        onComplete?.Invoke();
        Destroy(gameObject);
    }
}
