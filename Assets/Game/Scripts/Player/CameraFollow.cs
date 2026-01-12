using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new(0f, 20f, 0f);
    [SerializeField] private float smoothTime = 0.15f;
    private const float CAMERA_ROTATION_X = 90f;
    private Vector3 velocity;

    private void LateUpdate() {
        if (target == null) {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) target = player.transform;
            else return;
        }
        Vector3 desired = target.position + offset;
        transform.SetPositionAndRotation(Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime), Quaternion.Euler(CAMERA_ROTATION_X, 0f, 0f));
    }
}
