using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 position = _target.position;

        // 플레이어의 X Y만 따라가고 카메라 깊이는 유지한댜.
        position.z = transform.position.z;
        transform.position = position;
    }
}