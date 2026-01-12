using UnityEngine;

public class EnemyOrbiter : EnemyBase {
    [SerializeField] private float orbitRadius = 5f;
    [SerializeField] private float orbitSpeed = 90f;
    [SerializeField] private float approachRate = 0.5f;
    private float angle;
    protected override void InitializeEnemyType() => Type = EnemyType.Orbiter;
    protected override Vector3 GetMoveDirection() {
        if (player == null) return Vector3.zero;
        orbitRadius = Mathf.Max(0.5f, orbitRadius - approachRate * Time.fixedDeltaTime);
        angle += orbitSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime;
        Vector3 center = player.position;
        Vector3 orbitPos = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * orbitRadius;
        Vector3 toOrbit = orbitPos - transform.position;
        toOrbit.y = 0f;
        return toOrbit.normalized;
    }
}
