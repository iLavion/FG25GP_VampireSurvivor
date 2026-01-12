using UnityEngine;

public class ParticleEffectManager : MonoBehaviour {
    public static ParticleEffectManager Instance { get; private set; }
    [SerializeField] private ParticleSystem deathParticlePrefab;
    [SerializeField] private int particlePoolSize = 20;
    private ParticleSystem[] particlePool;
    private int currentPoolIndex;
    private void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePool();
    }
    private void InitializePool() {
        if (deathParticlePrefab == null) return;
        particlePool = new ParticleSystem[particlePoolSize];
        for (int i = 0; i < particlePoolSize; i++) {
            var instance = Instantiate(deathParticlePrefab, transform);
            instance.gameObject.SetActive(false);
            particlePool[i] = instance;
        }
    }
    public void PlayDeathEffect(Vector3 position) {
        if (particlePool == null || particlePool.Length == 0) return;
        ParticleSystem effect = particlePool[currentPoolIndex];
        currentPoolIndex = (currentPoolIndex + 1) % particlePoolSize;
        effect.transform.position = position;
        effect.gameObject.SetActive(true);
        effect.Play();
    }
}
