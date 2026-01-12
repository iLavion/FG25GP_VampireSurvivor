using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour {
    [SerializeField] private Projectile prefab;
    [SerializeField] private int poolSize = 50;
    private readonly Queue<Projectile> pool = new();
    private Transform parent;
    private int currentPoolSize;
    private void Start() { parent = transform; if (prefab == null) return; InitializePool(poolSize); }
    private void InitializePool(int size) {
        currentPoolSize = size;
        for (int i = 0; i < size; i++) {
            var inst = Instantiate(prefab, parent);
            inst.gameObject.SetActive(false);
            pool.Enqueue(inst);
        }
    }
    public Projectile Get() {
        if (prefab == null) return null;
        Projectile inst = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab, parent);
        inst.gameObject.SetActive(true);
        return inst;
    }
    public void Release(Projectile inst) { if (inst == null) return; inst.transform.SetParent(parent); inst.gameObject.SetActive(false); pool.Enqueue(inst); }
    public void RecalculatePoolSize(float attackCooldown, float projectileLifetime) {
        if (prefab == null) return;
        float projectilesPerSecond = 1f / Mathf.Max(0.01f, attackCooldown);
        int maxConcurrent = Mathf.CeilToInt(projectilesPerSecond * projectileLifetime);
        int targetPoolSize = Mathf.CeilToInt(maxConcurrent * 1.1f);
        if (targetPoolSize <= currentPoolSize) return;
        int toAdd = targetPoolSize - currentPoolSize;
        for (int i = 0; i < toAdd; i++) {
            var inst = Instantiate(prefab, parent);
            inst.gameObject.SetActive(false);
            pool.Enqueue(inst);
        }
        currentPoolSize = targetPoolSize;
    }
}
