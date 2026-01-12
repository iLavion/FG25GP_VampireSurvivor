using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class EnemyHealth : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Vector3 offset = new(0f, 2.5f, 0.2f);
    private Health health;
    private Transform target;
    private Camera mainCamera;
    private void Awake() => mainCamera = Camera.main;
    public void Initialize(Health targetHealth, Transform targetTransform) {
        Cleanup();
        health = targetHealth;
        target = targetTransform;
        if (health != null) {
            health.onHealthChanged.AddListener(OnHealthChanged);
            OnHealthChanged(health.Current, health.Max);
        }
    }
    private void OnDestroy() => Cleanup();
    private void Cleanup() {
        if (health != null) health.onHealthChanged.RemoveListener(OnHealthChanged);
        health = null;
        target = null;
    }
    private void LateUpdate() {
        if (target == null || mainCamera == null || healthText == null) return;
        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        GetComponent<RectTransform>().position = screenPos;
        bool inFront = Vector3.Dot(mainCamera.transform.forward, worldPos - mainCamera.transform.position) > 0;
        gameObject.SetActive(inFront);
    }
    private void OnHealthChanged(float current, float max) {
        if (healthText != null) healthText.text = $"{Mathf.RoundToInt(current)}/{Mathf.RoundToInt(max)}";
    }
}
