using UnityEngine;

public class SlimeMotion : MonoBehaviour
{
    private enum MotionState
    {
        Prepare,    // 정지 - 서서히 납작하게 압축
        Jump,       // 도약 - 위로 솟았다 내려오며 세로로 늘어남
        Land        // 정지 - 가로로 퍼졌다가 원래 크기로 복원
    }

    [SerializeField] private Transform _visual;

    [SerializeField, Min(0.01f)]
    private float _prepareDuration = 0.15f;

    [SerializeField, Min(0.01f)]
    private float _jumpDuration = 0.3f;

    [SerializeField, Min(0.01f)]
    private float _landDuration = 0.12f;

    [SerializeField] private float _hopHeight = 0.7f;

    [SerializeField, Range(0f, 0.3f)]
    private float _squashAmount = 0.2f;

    [SerializeField] private int _dustParticleCount = 15;
    [SerializeField] private Vector2 _dustOffset = new Vector2(0f, -0.15f); // 월드 좌표 기준 발밑 위치 보정값

    private Enemy _enemy;
    private Rigidbody2D _rd;

    private Vector3 _basePosition;
    private Vector3 _baseScale;

    private MotionState _state;
    private float _stateTime;

    private Vector2 _jumpDirection;
    private float _jumpSpeed;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rd = GetComponent<Rigidbody2D>();

        _basePosition = _visual.localPosition;
        _baseScale = _visual.localScale;

        // 첫 도약 시점을 조금씩 다르게 한다.
        _stateTime = -Random.Range(0f, _prepareDuration);
    }

    private void FixedUpdate()
    {
        // 준비·착지 중에는 이전 이동 속도가 남지 않도록 한다.
        _rd.linearVelocity = Vector2.zero;

        if (_enemy.Target == null)
        {
            ChangeState(MotionState.Prepare);
            SetVisual(0f, 0f);
            return;
        }

        _stateTime += Time.fixedDeltaTime;

        switch (_state)
        {
            case MotionState.Prepare:
                Prepare();
                break;

            case MotionState.Jump:
                Jump();
                break;

            case MotionState.Land:
                Land();
                break;
        }
    }

    private void Prepare()
    {
        float progress =
            Mathf.Clamp01(_stateTime / _prepareDuration);

        SetVisual(_squashAmount * progress, 0f);

        if (_stateTime < _prepareDuration)
            return;

        Vector2 offset =
            (Vector2)_enemy.Target.position - _rd.position;

        _jumpDirection = offset.normalized;

        // 출발 시 대상까지의 거리를 넘어가지 않도록 제한한다.
        _jumpSpeed = Mathf.Min(
            _enemy.MoveSpeed,
            offset.magnitude / _jumpDuration);

        ChangeState(MotionState.Jump);
    }

    private void Jump()
    {
        float progress =
            Mathf.Clamp01(_stateTime / _jumpDuration);

        float arc = Mathf.Sin(progress * Mathf.PI);

        // 출발·착지 때 압축되고, 공중에서는 세로로 늘어난다.
        float squash = _squashAmount * (1f - 2f * arc);

        SetVisual(squash, arc * _hopHeight);

        float remainingTime =
            _jumpDuration - (_stateTime - Time.fixedDeltaTime);

        float moveTime = Mathf.Clamp(
            remainingTime, 0f, Time.fixedDeltaTime);

        _rd.MovePosition(
            _rd.position + _jumpDirection * _jumpSpeed * moveTime);

        if (_stateTime >= _jumpDuration)
        {
            // 간단하게 상태머신을 구현해봤음
            ChangeState(MotionState.Land);
            SpawnLandingDust();
        }
    }

    private void Land()
    {
        float progress =
            Mathf.Clamp01(_stateTime / _landDuration);

        SetVisual(_squashAmount * (1f - progress), 0f);

        if (_stateTime >= _landDuration)
        {
            ChangeState(MotionState.Prepare);
        }
    }

    private void ChangeState(MotionState state)
    {
        _state = state;
        _stateTime = 0f;
    }

    private void SetVisual(float squash, float height)
    {
        _visual.localPosition =
            _basePosition + Vector3.up * height;

        _visual.localScale = new Vector3(
            _baseScale.x * (1f + squash),
            _baseScale.y * (1f - squash),
            _baseScale.z);
    }

    // 먼지를 생성하는 함수
    private void SpawnLandingDust()
    {
        float worldHeight =
            _hopHeight * Mathf.Abs(transform.lossyScale.y);

        float landingSpeed =
            Mathf.PI * worldHeight / _jumpDuration;

        Vector2 position = _rd.position + _dustOffset;

        if (LandingDustPool.Instance == null) return;

        LandingDustPool.Instance.Play(
            position,
            landingSpeed,
            _dustParticleCount);
    }
}