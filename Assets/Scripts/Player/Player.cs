using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] 
    private EnemySpawner _enemySpawner;
    [SerializeField] 
    private float _moveSpeed = 1f;
    [SerializeField] 
    private float _avoidDistance = 2f;  // 유지할 목표 거리
    [SerializeField] 
    private float _distanceGain = 2f;  // 거리가 어긋났을 때 보정하는 강도
    [SerializeField] 
    private float _orbitSpeed = 2f;    // 적 주위를 도는 속도

    [SerializeField, Min(0f)]
    private float _avoidStopMargin = 0.3f;

    [SerializeField]
    private HitReaction _hitReaction;

    [SerializeField]
    private int _maxHP;
    [SerializeField]
    private int _maxMP;

    [SerializeField]
    private HealthBar _healthBar;

    [SerializeField]
    private string _playerName;

    [SerializeField]
    private PlayerInfo _playerInfo;

    private Vector2 _avoidVelocity;
 
    private PlayerState _player;
    public PlayerState PlayerState => _player;

    private EPlayerJob _job;
    public EPlayerJob Job => _job;

    private Rigidbody2D _rd;

    private Enemy _target;
    public Enemy Target => _target;

    private float _targetSearchTimer;
    private bool _isAvoiding;

    public bool IsDead { get; private set; }

    public bool IsMoving =>
        !IsDead && _avoidVelocity.sqrMagnitude > 0.0001f;

    public event Action Damaged; // 살아있는 상태에서 피해를 받을때 알림
    public event Action Died;    // 사망 알림

    private Vector3 _pos;

    private int _levelExp = 100;

    private void Awake()
    {
        _player.Name = _playerName;
        _player.Level = 1;
        _player.Exp = 0;
        _player.HP = _maxHP;
        _player.MP = _maxMP;
        _player.STR = 5;
        _player.DEX = 5;
        _player.CON = 5;
        _player.INT = 5;
        _player.WIS = 5;
        _player.LUK = 5;
        _player.AFF = 1;
        _job = EPlayerJob.EWizard;

        _rd = GetComponent<Rigidbody2D>();

        _healthBar.SetHealth(_player.HP, _maxHP);
        RefreshPlayerInfo();
    }

    private void Update()
    {
        if(IsDead)
        {
            return;
        }

        UpdateTarget();
        Avoid();
    }

    private void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }

        Vector2 nextPosition =
            _rd.position + _avoidVelocity * Time.fixedDeltaTime;

        _rd.MovePosition(nextPosition);
    }

    private void UpdateTarget()
    {
        if(_enemySpawner == null)
        {
            _target = null;
            return;
        }

        _targetSearchTimer -= Time.deltaTime;

        if (_targetSearchTimer > 0f)
        {
            return;
        }

        _target = _enemySpawner.FindNearestEnemy(_rd.position);
        _targetSearchTimer = 0.2f;
    }

    private void Avoid()
    {
        // 대상이 없을 때 이전 이동이 남지 않도록 초기화한다.
        _avoidVelocity = Vector2.zero;

        if (_target == null ||
            !_target.isActiveAndEnabled ||
            _target.IsDead)
        {
            _isAvoiding = false;
            return;
        }

        Vector2 toEnemy =
            (Vector2)_target.transform.position - _rd.position;

        float distance = toEnemy.magnitude;

        if (_isAvoiding)
        {
            if(distance >= _avoidDistance + _avoidStopMargin)
            {
                _isAvoiding = false;
                return;
            }
        }

        else
        {
            // 적과의 목표하는 거리보다 적과의 거리가 멀다면 무시
            if(distance >= _avoidDistance)
            {
                return;
            }

            _isAvoiding = true;
        }

        float maxSpeed = 
            Mathf.Max(0f, _moveSpeed + _player.DEX);

        // 안전거리 이상이면 이동하지 않는다.
        if (distance >= _avoidDistance)
        {
            return;
        }

        if (distance <= 0.0001f)
        {
            _avoidVelocity = Vector2.right * maxSpeed;
            return;
        }

        Vector2 forward = toEnemy / distance;
        Vector2 right = new Vector2(forward.y, -forward.x);

        float distanceError = 
            distance - _avoidDistance;

        Vector2 correction = 
            forward * distanceError * _distanceGain;

        Vector2 desiredVelocity = 
            right * _orbitSpeed + correction;

        _avoidVelocity = 
            Vector2.ClampMagnitude(
                desiredVelocity, 
                maxSpeed);
    }

    public void GainExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _player.Exp += amount;

        RefreshPlayerInfo();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || 
            _player.HP <= 0 || 
            damage <= 0)
        {
            return;
        }

        print($"플레이어가 {damage}만큼 공격당했습니다");

        _player.HP = Mathf.Max(0, _player.HP - damage);

        _healthBar.SetHealth(_player.HP, _maxHP);
        _hitReaction.Play();

        print($"플레이어의 남은 HP {_player.HP} / {_maxHP}");

        // HP가 소진됐는지 검사하는 위치
        if (_player.HP <= 0)
        {
            Die();
            return;
        }

        Damaged?.Invoke();
    }

    private void Die()
    {
        // 사망 처리를 이미 시작했는지 검사하는 위치
        if (IsDead)
        {
            return;
        }

        IsDead = true;

        _avoidVelocity = Vector2.zero;
        _isAvoiding = false;
        _target = null;

        _rd.linearVelocity = Vector2.zero;
        _rd.simulated
            = false;

        _healthBar.Hide();

        Died?.Invoke();
    }

    private void RefreshPlayerInfo()
    {
        if(_playerInfo == null)
        {
            return;
        }

        _playerInfo.SetInfo(
            _player.Name,
            _player.Level,
            _player.Exp,
            _levelExp);
    }

    public void TryLevelUp()
    {
        if(_player.Exp < _levelExp)
        {
            return;
        }

        _player.Exp -= _levelExp;
        _player.Level++;

        _levelExp = (int)(_levelExp * 1.1f);

        RefreshPlayerInfo();
    }
}