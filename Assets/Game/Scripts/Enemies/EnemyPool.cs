using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour {
    private readonly Dictionary<EnemyType, Queue<EnemyBase>> poolsByType = new();
    private readonly Dictionary<EnemyType, EnemyBase> prefabsByType = new();
    private Transform parent;
    private void Start() {
        parent = transform;
        if (GameManager.Instance == null) return;
        foreach (var cfg in GameManager.Instance.Enemies) {
            var queue = new Queue<EnemyBase>();
            for (int i = 0; i < cfg.poolSize; i++) {
                var inst = Instantiate(cfg.prefab, parent);
                inst.gameObject.SetActive(false);
                queue.Enqueue(inst);
            }
            poolsByType[cfg.type] = queue;
            prefabsByType[cfg.type] = cfg.prefab;
        }
    }
    public EnemyBase Get(EnemyType type, Vector3 position, float difficultyMultiplier = 1f) {
        if (!poolsByType.ContainsKey(type)) return null;
        var pool = poolsByType[type];
        EnemyBase inst;
        if (pool.Count > 0) inst = pool.Dequeue();
        else inst = Instantiate(prefabsByType[type], parent);
        inst.transform.SetPositionAndRotation(position, Quaternion.identity);
        inst.SetDifficultyMultiplier(difficultyMultiplier);
        inst.gameObject.SetActive(true);
        return inst;
    }
    public void Release(EnemyBase enemy) {
        if (enemy == null) return;
        enemy.gameObject.SetActive(false);
        EnemyType type = enemy.Type;
        if (poolsByType.ContainsKey(type)) poolsByType[type].Enqueue(enemy);
    }
}
