using UnityEngine;
using System.Collections.Generic;

public class DamagePopupPool : MonoBehaviour {
    public static DamagePopupPool Instance { get; private set; }
    [SerializeField] private DamagePopup prefab;
    [SerializeField] private int initialPoolSize = 50;
    private readonly Queue<DamagePopup> pool = new();
    private Canvas canvas;
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    private void Start() {
        if (GameManager.Instance != null) canvas = GameManager.Instance.GameHUDCanvas;
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
        if (prefab == null && GameManager.Instance != null) prefab = GameManager.Instance.DamagePopupPrefab;
        if (prefab == null) return;
        for (int i = 0; i < initialPoolSize; i++) {
            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }
    public DamagePopup Get() {
        DamagePopup instance;
        if (pool.Count > 0) instance = pool.Dequeue();
        else if (prefab != null) instance = Instantiate(prefab, transform);
        else return null;
        if (canvas != null) instance.transform.SetParent(canvas.transform, false);
        instance.gameObject.SetActive(true);
        return instance;
    }
    public void Release(DamagePopup instance) {
        if (instance == null) return;
        instance.ClearAllItems();
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform, false);
        pool.Enqueue(instance);
    }
}
