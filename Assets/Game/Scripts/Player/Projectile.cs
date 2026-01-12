using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask hitLayers;
    private Vector3 direction;
    private float timer;
    private ProjectilePool pool;
    private GameObject owner;
    public void Init(
        Vector3 dir, float spd,
        float dmg,
        float life,
        LayerMask layers,
        ProjectilePool sourcePool = null,
        GameObject shooter = null
    ) {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        lifetime = life;
        hitLayers = layers;
        pool = sourcePool;
        owner = shooter;
        timer = 0f;
    }
    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.IsPaused) return;
        transform.position += speed * Time.deltaTime * direction;
        timer += Time.deltaTime;
        if (timer >= lifetime) ReleaseOrDestroy();
    }
    private void OnTriggerEnter(Collider other) {
        if (owner != null && other.gameObject == owner) return;
        if (hitLayers.value != 0 && ((1 << other.gameObject.layer) & hitLayers.value) == 0) return;
        if (other.TryGetComponent<Health>(out var hp)) { hp.TakeDamage(damage); ReleaseOrDestroy(); }
    }
    private void OnCollisionEnter(Collision collision) {
        if (owner != null && collision.gameObject == owner) return;
        if (hitLayers.value != 0 && ((1 << collision.gameObject.layer) & hitLayers.value) == 0) return;
        if (collision.gameObject.TryGetComponent<Health>(out var hp)) { hp.TakeDamage(damage); ReleaseOrDestroy(); }
    }
    private void ReleaseOrDestroy() { if (pool != null) pool.Release(this); else Destroy(gameObject); }
}
