using UnityEngine;

[RequireComponent(typeof(Enemy), typeof(Rigidbody2D))]
public class SlimeMotion : MonoBehaviour
{
    // Attack 클립의 이륙·첫 착지 키 시간과 함께 유지한다.
    private const float AttackTakeoffTime = 0.18f;
    private const float AttackImpactTime = 0.68f;

    private enum MotionState
    {
        Idle,
        Prepare,
        Jump,
        Land,
        Attack
    }

    private static readonly int[] AnimationStates =
    {
        Animator.StringToHash("Base Layer.Idle"),
        Animator.StringToHash("Base Layer.Prepare"),
        Animator.StringToHash("Base Layer.Jump"),
        Animator.StringToHash("Base Layer.Land"),
        Animator.StringToHash("Base Layer.Attack")
    };

    [SerializeField] private Animator _animator;

    [SerializeField, Min(0.01f)]
    private float _prepareDuration = 0.15f;

    [SerializeField, Min(0.01f)]
    private float _jumpDuration = 0.3f;

    [SerializeField, Min(0.01f)]
    private float _landDuration = 0.12f;

    [SerializeField, Min(0.01f)]
    private float _attackDuration = 0.8f;

    [SerializeField, Min(0f)] private float _attackRange = 1.5f;
    [SerializeField, Min(0.01f)] private float _attackCooldown = 2f;
    [SerializeField, Min(1f)] private float _attackMoveSpeedMultiplier = 4f;
    [SerializeField, Min(0f)] private float _attackHopHeight = 1.4f;
    [SerializeField, Range(1f, 3f)] private float _attackDustSpread = 1.8f;
    [SerializeField, Range(1, 3)] private int _attackDustCountMultiplier = 2;

    // 실제 도약 클립의 최고 높이에 맞추는 먼지 연출 기준값.
    [SerializeField, Min(0f)]
    private float _hopHeight = 0.7f;

    [SerializeField, Min(1)]
    private int _dustParticleCount = 15;

    [SerializeField]
    private Vector2 _dustOffset = new Vector2(0f, -0.15f);

    private Enemy _enemy;
    private Rigidbody2D _rd;

    private MotionState _state;
    private float _stateTime;
    private float _stateDuration;

    private Vector2 _jumpDirection;
    private float _jumpSpeed;
    private float _attackCooldownRemaining;
    private bool _attackDustSpawned;

    private void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rd = GetComponent<Rigidbody2D>();

        if (_animator == null ||
            _animator.runtimeAnimatorController == null)
        {
            Debug.LogError(
                "SlimeMotion에 Animator와 Controller를 연결해야 합니다.",
                this);

            enabled = false;
        }
    }

    private void OnEnable()
    {
        ChangeState(MotionState.Idle);
        _attackCooldownRemaining = Random.Range(0f, _attackCooldown);
    }

    private void OnDisable()
    {
        if (_rd != null)
            _rd.linearVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        _rd.linearVelocity = Vector2.zero;
        _attackCooldownRemaining = Mathf.Max(
            0f, _attackCooldownRemaining - Time.fixedDeltaTime);

        if (_enemy.Target == null)
        {
            if (_state != MotionState.Idle)
                ChangeState(MotionState.Idle);

            return;
        }

        if (_state == MotionState.Idle)
        {
            ChangeState(MotionState.Prepare);
            return;
        }

        if (_state == MotionState.Jump)
        {
            float moveTime = Mathf.Min(
                Time.fixedDeltaTime,
                Mathf.Max(0f, _stateDuration - _stateTime));

            _rd.MovePosition(
                _rd.position +
                _jumpDirection * _jumpSpeed * moveTime);
        }
        else if (_state == MotionState.Attack)
        {
            // 압축·착지 반동 중에는 멈추고, 공중 구간에서만 전진한다.
            float flightStart = _stateDuration * AttackTakeoffTime;
            float flightEnd = _stateDuration * AttackImpactTime;
            float moveTime = Mathf.Max(
                0f,
                Mathf.Min(_stateTime + Time.fixedDeltaTime, flightEnd) -
                Mathf.Max(_stateTime, flightStart));

            if (moveTime > 0f)
            {
                _rd.MovePosition(
                    _rd.position + _jumpDirection * _jumpSpeed * moveTime);
            }
        }

        _stateTime += Time.fixedDeltaTime;

        if (_state == MotionState.Attack && !_attackDustSpawned &&
            _stateTime >= _stateDuration * AttackImpactTime)
        {
            _attackDustSpawned = true;
            SpawnLandingDust(true);
        }

        if (_stateTime < _stateDuration)
            return;

        switch (_state)
        {
            case MotionState.Prepare:
                Vector2 offset =
                    (Vector2)_enemy.Target.position - _rd.position;
                bool attack = _attackCooldownRemaining <= 0f &&
                    offset.sqrMagnitude <= _attackRange * _attackRange;
                BeginJump(attack);
                break;

            case MotionState.Jump:
                ChangeState(MotionState.Land);
                SpawnLandingDust();
                break;

            case MotionState.Land:
                ChangeState(MotionState.Prepare);
                break;

            case MotionState.Attack:
                // Attack 클립에 착지·잔반동까지 포함되어 있다.
                ChangeState(MotionState.Prepare);
                break;
        }
    }

    private void BeginJump(bool attack)
    {
        Vector2 offset =
            (Vector2)_enemy.Target.position - _rd.position;

        _jumpDirection = offset.normalized;

        float duration = attack
            ? _attackDuration * (AttackImpactTime - AttackTakeoffTime)
            : _jumpDuration;
        float speedMultiplier = attack ? _attackMoveSpeedMultiplier : 1f;

        // 도약 시작 때 방향을 고정하고 당시 타깃 위치를 넘지 않게 한다.
        _jumpSpeed = Mathf.Min(
            Mathf.Max(0f, _enemy.MoveSpeed) * speedMultiplier,
            offset.magnitude / Mathf.Max(0.01f, duration));

        if (attack)
        {
            _attackCooldownRemaining = _attackCooldown;
            _attackDustSpawned = false;
        }

        ChangeState(attack ? MotionState.Attack : MotionState.Jump);
    }

    private void ChangeState(MotionState state)
    {
        _state = state;
        _stateTime = 0f;

        float duration = state switch
        {
            MotionState.Prepare => _prepareDuration,
            MotionState.Jump => _jumpDuration,
            MotionState.Land => _landDuration,
            MotionState.Attack => _attackDuration,
            _ => 1f
        };

        _stateDuration = Mathf.Max(0.01f, duration);

        // 현재 프로젝트의 모든 클립이 1초라는 전제.
        _animator.speed = 1f / _stateDuration;
        _animator.Play(AnimationStates[(int)state], 0, 0f);
    }

    private void SpawnLandingDust(bool attack = false)
    {
        if (LandingDustPool.Instance == null)
            return;

        float height = attack ? _attackHopHeight : _hopHeight;
        float duration = attack
            ? _attackDuration * (AttackImpactTime - AttackTakeoffTime)
            : _jumpDuration;
        float worldHeight = height * Mathf.Abs(transform.lossyScale.y);

        // 실제 충돌 속도가 아닌 먼지 연출용 기준값.
        float strength =
            Mathf.PI * worldHeight /
            Mathf.Max(0.01f, duration);

        LandingDustPool.Instance.Play(
            _rd.position + _dustOffset,
            strength,
            _dustParticleCount * (attack ? _attackDustCountMultiplier : 1),
            attack ? _attackDustSpread : 1f);
    }
}
