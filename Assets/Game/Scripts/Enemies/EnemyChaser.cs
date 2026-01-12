using UnityEngine;

public class EnemyChaser : EnemyBase {
    protected override void InitializeEnemyType() => Type = EnemyType.Chaser;
    protected override Vector3 GetMoveDirection() {
        if (player == null) return Vector3.zero;
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        return toPlayer.normalized;
    }
}
