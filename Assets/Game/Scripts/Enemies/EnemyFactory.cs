using UnityEngine;

public enum EnemyType { Chaser, Orbiter, Boss }

public class EnemyFactory : MonoBehaviour {
    [SerializeField] private EnemyPool pool;
    private void Start() { if (pool == null) pool = FindFirstObjectByType<EnemyPool>(); }
    public EnemyBase Create(EnemyType type, Vector3 position, float difficultyMultiplier = 1f) {
        if (pool == null) return null;
        EnemyBase enemy = pool.Get(type, position, difficultyMultiplier);
        if (enemy != null) enemy.SetPool(pool);
        return enemy;
    }
}
