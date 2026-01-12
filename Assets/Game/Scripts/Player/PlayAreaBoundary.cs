using UnityEngine;

public class PlayAreaBoundary : MonoBehaviour {
    [SerializeField] private float boundaryRadius = 15f;
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField] private bool visualizeInEditor = true;

    private void LateUpdate() {
        Vector3 pos = transform.position;
        Vector3 centerPos = new(center.x, pos.y, center.z);
        Vector3 horizontalOffset = pos - centerPos;
        float distance = new Vector2(horizontalOffset.x, horizontalOffset.z).magnitude;
        if (distance > boundaryRadius) {
            Vector3 direction = horizontalOffset.normalized;
            Vector3 clampedPos = centerPos + direction * boundaryRadius;
            transform.position = clampedPos;
        }
    }
    private void OnDrawGizmos() {
        if (!visualizeInEditor) return;
        Gizmos.color = Color.yellow;
        int segments = 64;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(boundaryRadius, 0, 0);
        for (int i = 1; i <= segments; i++) {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * boundaryRadius, 0, Mathf.Sin(angle) * boundaryRadius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
