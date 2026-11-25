using UnityEngine;
using System.Collections;

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

    [SerializeField, Min(0f)] 
    private float _attackRange = 1.5f;
    [SerializeField, Min(0.01f)] 
    private float _attackCooldown = 2f;
    [SerializeField, Min(1f)] 
    private float _attackMoveSpeedMultiplier = 4f;
    [SerializeField, Min(0f)] 
    private float _attackHopHeight = 1.4f;
    [SerializeField, Range(1f, 3f)] 
    private float _attackDustSpread = 1.8f;
    [SerializeField, Range(1, 3)] 
    private int _attackDustCountMultiplier = 2;

    // 실제 도약 클립의 최고 높이에 맞추는 먼지 연출 기준값.
    [SerializeField, Min(0f)]
    private float _hopHeight = 0.7f;

    [SerializeField, Min(1)]
    private int _dustParticleCount = 15;

    [SerializeField]
    private Vector2 _dustOffset = new Vector2(0f, -0.15f);

    [SerializeField]
    private float _attackHitRange = 0.6f;

    [SerializeField]
    private AnimationClip _deathClip;

    [SerializeField]
    private GameObject _groundShadow;

    private Enemy _enemy;
    private Rigidbody2D _rd;

    private MotionState _state;
    private float _stateTime;
    private float _stateDuration;

    private Vector2 _jumpDirection;
    private float _jumpSpeed;
    private float _attackCooldownRemaining;
    private bool _attackDustSpawned;

    private Vector2 _knockbackVelocity;
    private float _knockbackTimeRemaining;

    private static readonly int DeathState =
        Animator.StringToHash("Base Layer.Death");

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
        _enemy.Died += PlayDeath;

        ChangeState(MotionState.Idle);
        _attackCooldownRemaining = Random.Range(0f, _attackCooldown);
    }

    private void OnDisable()
    {
        _knockbackTimeRemaining = 0f;

        if(_enemy != null)
        {
            _enemy.Died -= PlayDeath;
        }

        if (_rd != null)
        {
            _rd.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!_enemy.CanAct)
        {
            return;
        }

        _rd.linearVelocity = Vector2.zero;
        _attackCooldownRemaining = Mathf.Max(
            0f, _attackCooldownRemaining - Time.fixedDeltaTime);

        if (_enemy.Target == null)
        {
            if (_state != MotionState.Idle)
            {
                ChangeState(MotionState.Idle);
            }

            MoveWithKnockback(Vector2.zero);
            return;
        }

        if (_state == MotionState.Idle)
        {
            ChangeState(MotionState.Prepare);
            MoveWithKnockback(Vector2.zero);
            return;
        }

        Vector2 movement = Vector2.zero;

        if (_state == MotionState.Jump)
        {
            float moveTime = Mathf.Min(
                Time.fixedDeltaTime,
                Mathf.Max(0f, _stateDuration - _stateTime));

            movement = _jumpDirection * _jumpSpeed * moveTime;
        }
        else if (_state == MotionState.Attack)
        {
            // 압축과 착지 반동 사이의 공중 구간에서 전진한다.
            float flightStart = _stateDuration * AttackTakeoffTime;
            float flightEnd = _stateDuration * AttackImpactTime;
            float moveTime = Mathf.Max(
                0f,
                Mathf.Min(_stateTime + Time.fixedDeltaTime, flightEnd) -
                Mathf.Max(_stateTime, flightStart));

            if (moveTime > 0f)
            {
                // 보스는 실제 이륙 시점에 착지 방향을 확정한다.
                if (_enemy.EnemyState.IsBoss && _stateTime <= flightStart)
                {
                    SetJumpTrajectory(
                        flightEnd - flightStart, _attackMoveSpeedMultiplier);
                }

                movement = _jumpDirection * _jumpSpeed * moveTime;
            }
        }

        // 도약 이동과 넉백을 합쳐 물리 프레임당 한 번 이동한다.
        MoveWithKnockback(movement);
        _stateTime += Time.fixedDeltaTime;

        if (_state == MotionState.Attack && !_attackDustSpawned &&
            _stateTime >= _stateDuration * AttackImpactTime)
        {
            _attackDustSpawned = true;

            TryHitPlayer();
            SpawnLandingDust(true);
        }

        if (_stateTime < _stateDuration)
        {
            return;
        }

        switch (_state)
        {
            case MotionState.Prepare:
                Vector2 offset = (Vector2)_enemy.Target.position - _rd.position;
                bool attack = _attackCooldownRemaining <= 0f &&
                    offset.sqrMagnitude <= _attackRange * _attackRange;
                BeginJump(attack);
                break;

            case MotionState.Jump:
                ChangeState(MotionState.Land);
                SpawnLandingDust();
                break;

            case MotionState.Land:
            case MotionState.Attack:
                ChangeState(MotionState.Prepare);
                break;
        }
    }

    private void BeginJump(bool attack)
    {
        float duration = attack
            ? _attackDuration * (AttackImpactTime - AttackTakeoffTime)
            : _jumpDuration;
        float speedMultiplier = attack ? _attackMoveSpeedMultiplier : 1f;

        SetJumpTrajectory(duration, speedMultiplier);

        if (attack)
        {
            _attackCooldownRemaining = _attackCooldown;
            _attackDustSpawned = false;
        }

        ChangeState(attack ? MotionState.Attack : MotionState.Jump);
    }

    private void SetJumpTrajectory(float duration, float speedMultiplier)
    {
        Vector2 offset = (Vector2)_enemy.Target.position - _rd.position;
        _jumpDirection = offset.normalized;

        // 도약 중 목표 지점을 지나치지 않도록 속도를 제한한다.
        _jumpSpeed = Mathf.Min(
            Mathf.Max(0f, _enemy.MoveSpeed) * speedMultiplier,
            offset.magnitude / Mathf.Max(0.01f, duration));
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
            _enemy.EnemyState.Element,
            _rd.position + _dustOffset,
            strength,
            _dustParticleCount * (attack ? _attackDustCountMultiplier : 1),
            attack ? _attackDustSpread : 1f);
    }

    private void TryHitPlayer()
    {
        Transform target = _enemy.Target;

        if(target == null)
        {
            return;
        }

        Vector2 offset =
            (Vector2)target.position - _rd.position;

        if(offset.sqrMagnitude > _attackHitRange * _attackHitRange)
        {
            return;
        }

        if(target.TryGetComponent<Player>(out var player))
        {
            player.TakeDamage(_enemy.EnemyState.Attack);
        }
    }

    private void PlayDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    // Death 재생 → 클립 길이만큼 대기 → 제거를 순서대로 표현하기 쉬워
    // 코루틴을 사용한다. 기다리는 동안 다른 게임 동작은 계속 진행된다.
    // 실제 종료를 감지하는 방식은 아니므로 재생 클립과 _deathClip이 같고,
    // Animator 재생 속도가 1이라는 전제가 필요하다.

    // 대안:
    // - Animation Event: 클립의 지정 시점에서 제거 함수를 호출한다.
    // - 상태 타이머: Death 상태에서 시간을 누적하고 종료 시 제거한다.
    // - Awaitable: Unity 6의 async/await로 대기 흐름을 표현한다.
    private IEnumerator DeathRoutine()
    {
        if (_groundShadow != null)
        {
            _groundShadow.SetActive(false);
        }

        if (_deathClip != null)
        {
            _animator.speed = 1f;
            _animator.Play(DeathState, 0, 0f);

            yield return new WaitForSeconds(_deathClip.length);
        }

        // 오브젝트를 제거하기 전에 사망 모션 종료를 전달한다.
        _enemy.NotifyDeathMotionFinished();
        Destroy(gameObject);
    }

    public bool TryKnockback(Vector2 direction, float speed, float duration)
    {
        if (!isActiveAndEnabled || !_enemy.CanAct || speed <= 0f || duration <= 0f)
        {
            return false;
        }

        _knockbackVelocity = direction * speed;
        _knockbackTimeRemaining = duration;
        return true;
    }

    private void MoveWithKnockback(Vector2 movement)
    {
        float stepTime = Mathf.Min(_knockbackTimeRemaining, Time.fixedDeltaTime);
        _knockbackTimeRemaining -= stepTime;

        // 기존 도약 이동과 넉백을 합쳐 물리 프레임당 한 번 이동한다.
        Vector2 displacement = movement + _knockbackVelocity * stepTime;
        if (displacement.sqrMagnitude > 0f)
        {
            _rd.MovePosition(_rd.position + displacement);
        }
    }
}