using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private const float ShieldExpireEffectDuration = 0.33f;
    private static readonly int ShieldExpireTrigger = Animator.StringToHash("Expire");

    [SerializeField]
    private Animator _shieldEffectPrefab;

    [SerializeField]
    private GameObject _shieldBlockEffectPrefab;

    [SerializeField] 
    private EnemySpawner _enemySpawner;
    [SerializeField] 
    private float _moveSpeed = 1f;

    [SerializeField]
    private HitReaction _hitReaction;

    [SerializeField]
    private int _maxHP;
    [SerializeField]
    private int _maxMP;

    [SerializeField]
    private HealthBar _healthBar;

    [SerializeField]
    private PlayerInfo _playerInfo;

    [SerializeField]
    private Stage _stage;

    [SerializeField, Min(0)] 
    private int _statUpgradeCost;

    [SerializeField, Min(0)] 
    private int _statUpgradeIncrease;

    [SerializeField]
    private float _playerMoveSpeed;

    public int MaxHP => _maxHP;
    public int MaxMP => _maxMP;

    public event Action ProgressionChanged;

    public event Action Revived;
    public event Action LeveledUp;

    public bool CanAct =>
        !IsDead &&
        _stage != null &&
        _stage.CanFight;

    private PlayerState _player;
    public PlayerState PlayerState => _player;

    private EPlayerJob _job;
    public EPlayerJob Job => _job;

    private Rigidbody2D _rd;

    private Enemy _target;
    public Enemy Target => _target;

    private float _targetSearchTimer;
    private PlayerSkill _playerSKill;

    public bool IsDead { get; private set; }
    public bool IsLevelUpCardOpen { get; private set; }

    public bool IsMoving =>
        CanAct && _moveVelocity.sqrMagnitude > 0.0001f;

    public event Action Damaged; // 살아있는 상태에서 피해를 받을때 알림
    public event Action Died;    // 사망 알림

    private Vector3 _pos;

    private int _levelExp = 100;

    private InputAction _moveAction;
    private Vector2 _moveVelocity;
    private float _shieldExpiresAt;
    private Animator _shieldEffect;

    private void Awake()
    {
        _player.Level = 1;
        _player.Exp = 0;
        _player.Gold = 0;
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
        _playerSKill = GetComponent<PlayerSkill>();

        _moveAction = InputSystem.actions?.FindAction("Player/Move");

        if (_moveAction == null)
        {
            Debug.LogError("프로젝트 입력 에셋의 Player/Move를 확인하세요.", this);
        }

        _healthBar.SetHealth(_player.HP, _maxHP);
        RefreshPlayerInfo();
    }

    private void Update()
    {
        UpdateShieldEffect();

        if (!CanAct ||
            IsLevelUpCardOpen)
        {
            // 사망, 재시작, 보스 등장 중에는 이동 입력을 비운다.
            _moveVelocity = Vector2.zero;
            return;
        }

        UpdateTarget();

        Vector2 input = _moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        float speed = Mathf.Max(0f, _moveSpeed + _player.DEX);

        // 대각선 속도를 제한하면서 스틱을 조금 밀었을 때의 입력 세기는 유지한다.
        _moveVelocity = Vector2.ClampMagnitude(input, 1f) * speed;
    }

    private void FixedUpdate()
    {
        if (!CanAct)
        {
            return;
        }

        Vector2 nextPosition = _rd.position + _moveVelocity * Time.fixedDeltaTime;
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

    public void GainExperience(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _player.Exp += amount;

        RefreshPlayerInfo();
    }

    public void GainGold(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        _player.Gold += amount;
        ProgressionChanged?.Invoke();
    }

    public int GetStatUpgradeCost(PlayerStat stat)
    {
        return Enum.IsDefined(typeof(PlayerStat), stat) ? _statUpgradeCost : 0;
    }

    public bool CanUpgradeStat(PlayerStat stat)
    {
        int cost = GetStatUpgradeCost(stat);
        return cost > 0 && _statUpgradeIncrease > 0 && _player.Gold >= cost;
    }

    public bool TryUpgradeStat(PlayerStat stat)
    {
        if (!CanUpgradeStat(stat))
        {
            return false;
        }

        int cost = GetStatUpgradeCost(stat);
        int increase = _statUpgradeIncrease;

        switch (stat)
        {
            case PlayerStat.MaxHP:
                _maxHP += increase;
                break;
            case PlayerStat.MaxMP:
                _maxMP += increase;
                break;
            case PlayerStat.STR:
                _player.STR += increase;
                break;
            case PlayerStat.DEX:
                _player.DEX += increase;
                break;
            case PlayerStat.CON:
                _player.CON += increase;
                break;
            case PlayerStat.INT:
                _player.INT += increase;
                break;
            case PlayerStat.WIS:
                _player.WIS += increase;
                break;
            case PlayerStat.LUK:
                _player.LUK += increase;
                break;
            case PlayerStat.AFF:
                _player.AFF += increase;
                break;
            default:
                return false;
        }

        _player.Gold -= cost;

        // 최대 HP가 바뀌면 현재 HP 기준으로 체력바 비율을 다시 계산한다.
        if (stat == PlayerStat.MaxHP)
        {
            _healthBar.SetHealth(_player.HP, _maxHP);
        }

        ProgressionChanged?.Invoke();
        return true;
    }

    public void TakeDamage(int damage)
    {
        if (!CanAct || 
            _player.HP <= 0 || 
            damage <= 0)
        {
            return;
        }

        // 보호막이 유효하면 이번 피해를 막고 즉시 소모한다.
        if (Time.time < _shieldExpiresAt)
        {
            ClearShield();
            if (_shieldBlockEffectPrefab != null)
            {
                Instantiate(_shieldBlockEffectPrefab, transform.position, Quaternion.identity);
            }

            return;
        }

        _player.HP = Mathf.Max(0, _player.HP - damage);

        _healthBar.SetHealth(_player.HP, _maxHP);
        _hitReaction.Play();

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
        if (IsDead)
        {
            return;
        }

        IsDead = true;
        ClearShield();
        _moveVelocity = Vector2.zero;
        _target = null;

        _rd.linearVelocity = Vector2.zero;
        _rd.simulated = false;

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
            _player.Level,
            _player.Exp,
            _levelExp);
    }

    public void TryLevelUp()
    {
        if (!CanAct || 
            IsLevelUpCardOpen || 
            _player.Exp < _levelExp)
        {
            return;
        }

        // 선택할 강화가 없으면 경험치와 레벨을 보존한다.
        if(_playerSKill == null ||
            !_playerSKill.HasAvailableUpgrade)
        {
            return;
        }

        _player.Exp -= _levelExp;
        _player.Level++;
        _levelExp = (int)(_levelExp * 1.1f);

        RefreshPlayerInfo();
        LeveledUp?.Invoke();
    }

    public void Revive(Vector2 spawnPosition)
    {
        if (!IsDead)
        {
            return;
        }

        _player.HP = _maxHP;
        _moveVelocity = Vector2.zero;
        _target = null;
        _targetSearchTimer = 0f;

        // 물리 시뮬레이션을 다시 켜기 전에 위치와 속도를 복구한다.
        _rd.position = spawnPosition;
        _rd.linearVelocity = Vector2.zero;
        _rd.angularVelocity = 0f;
        _rd.simulated = true;

        IsDead = false;

        _hitReaction.ResetEffect();
        _healthBar.Show();
        _healthBar.SetHealth(_player.HP, _maxHP);

        Revived?.Invoke();
    }

    public void SetLevelUpCardOpen(bool isOpen)
    {
        IsLevelUpCardOpen = isOpen;

        if(isOpen)
        {
            _moveVelocity = Vector2.zero;
        }
    }

    public bool TryGrantShield(float duration)
    {
        if (!CanAct ||
            IsLevelUpCardOpen ||
            duration <= 0f ||
            Time.time < _shieldExpiresAt)
        {
            return false;
        }

        UpdateShieldEffect();
        _shieldExpiresAt = Time.time + duration;
        if (_shieldEffectPrefab != null)
        {
            _shieldEffect = Instantiate(_shieldEffectPrefab, transform.position, Quaternion.identity);
            _shieldEffect.transform.SetParent(transform, true);
        }

        return true;
    }

    private void UpdateShieldEffect()
    {
        if (_shieldExpiresAt <= 0f || Time.time < _shieldExpiresAt)
        {
            return;
        }

        _shieldExpiresAt = 0f;
        if (_shieldEffect != null)
        {
            _shieldEffect.SetTrigger(ShieldExpireTrigger);
            Destroy(_shieldEffect.gameObject, ShieldExpireEffectDuration);
            _shieldEffect = null;
        }
    }

    private void ClearShield()
    {
        _shieldExpiresAt = 0f;
        if (_shieldEffect != null)
        {
            _shieldEffect.gameObject.SetActive(false);
            Destroy(_shieldEffect.gameObject);
            _shieldEffect = null;
        }
    }

    private void OnDisable()
    {
        ClearShield();
    }
}