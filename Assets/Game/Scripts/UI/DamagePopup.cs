using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class DamagePopup : MonoBehaviour {
    [SerializeField] private DamagePopupItem itemPrefab;
    [SerializeField] private Vector3 offset = new(0f, 3.5f, 0.2f);
    [SerializeField] private int maxItems = 5;
    private Transform target;
    private Camera mainCamera;
    private readonly List<DamagePopupItem> items = new();
    private const float ITEM_SPACING = 24f;
    private void Awake() => mainCamera = Camera.main;
    public void Initialize(Transform targetTransform) { target = targetTransform; }
    public void ShowDamage(float damage) {
        if (itemPrefab == null) return;
        while (items.Count >= maxItems && items.Count > 0) {
            var oldest = items[0];
            items.RemoveAt(0);
            if (oldest != null) Destroy(oldest.gameObject);
        }
        var item = Instantiate(itemPrefab, transform);
        items.Add(item);
        item.Initialize(damage, () => { items.Remove(item); UpdateLayout(); });
        UpdateLayout();
    }
    private void LateUpdate() {
        if (target == null || mainCamera == null) return;
        Vector3 worldPos = target.position + offset;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        GetComponent<RectTransform>().position = screenPos;
        bool inFront = Vector3.Dot(mainCamera.transform.forward, worldPos - mainCamera.transform.position) > 0;
        gameObject.SetActive(inFront);
    }
    private void UpdateLayout() {
        for (int i = 0; i < items.Count; i++) {
            var rect = items[i].GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0f, -i * ITEM_SPACING);
        }
    }
    public void ClearAllItems() {
        for (int i = items.Count - 1; i >= 0; i--) if (items[i] != null) Destroy(items[i].gameObject);
        items.Clear();
    }
}
