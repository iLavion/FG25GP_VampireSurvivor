using UnityEngine;
using System.Collections.Generic;

public class EnemyHealthPool : MonoBehaviour {
    public static EnemyHealthPool Instance { get; private set; }
    [SerializeField] private EnemyHealth prefab;
    [SerializeField] private int initialPoolSize = 50;
    private readonly Queue<EnemyHealth> pool = new();
    private Canvas canvas;
    private void Awake() {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start() {
        if (GameManager.Instance != null) canvas = GameManager.Instance.GameHUDCanvas;
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();
        if (prefab == null && GameManager.Instance != null) prefab = GameManager.Instance.EnemyHealthPrefab;
        if (prefab == null) return;
        for (int i = 0; i < initialPoolSize; i++) {
            var instance = Instantiate(prefab, transform);
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }
    public EnemyHealth Get() {
        EnemyHealth instance;
        if (pool.Count > 0) instance = pool.Dequeue();
        else if (prefab != null) instance = Instantiate(prefab, transform);
        else return null;
        if (canvas != null) instance.transform.SetParent(canvas.transform, false);
        instance.gameObject.SetActive(true);
        return instance;
    }
    public void Release(EnemyHealth instance) {
        if (instance == null) return;
        instance.gameObject.SetActive(false);
        instance.transform.SetParent(transform, false);
        pool.Enqueue(instance);
    }
}
