using System;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{ 
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private float _attackInterval = 1f;
    [SerializeField] private Player _player;
    [SerializeField] private Transform _firePos;

    private Enemy _target;
    private float _attackTimer;
    private bool _hasFired;

    private int _baseMinDamage = 5;
    private int _baseMaxDamage = 10;

    public bool IsAttacking { get; private set; }
    public event Action AttackStarted;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        if (_player != null)
        {
            _player.Died += FinishAttack;
        }
    }

    private void OnDisable()
    {
        if (_player != null)
        {
            _player.Died -= FinishAttack;
        }

        FinishAttack();
    }

    private void Update()
    {
        if(_player.IsDead)
        {
            return;
        }

        _attackTimer -= Time.deltaTime;

        if(IsAttacking ||
            _attackTimer > 0f)
        {
            return;
        }

        // PlayerMotion이 연결된 후 공격을 시작
        if (AttackStarted == null)
        {
            return;
        }

        Enemy target = _player.Target;

        if (target == null || 
            !target.isActiveAndEnabled ||
            target.IsDead)
        {
            return;
        }

        if (_attackTimer > 0f)
        {
            return;
        }

        Vector2 direction = 
            target.transform.position - transform.position;

        if (direction.sqrMagnitude > _attackRange * _attackRange)
        {
            return;
        }

        // 공격 도중 검색 대상이 바뀌어도 이번 공격 대상은 유지
        _target = target;
        _hasFired = false;
        IsAttacking = true;
        _attackTimer = _attackInterval;

        AttackStarted.Invoke();
    }

    // PlayerMotion이 애니메이션 발사 시점에 호출
    public void MagicAttack()
    {
        if (_player.IsDead || !IsAttacking || _hasFired)
        {
            return;
        }

        // 같은 공격에서 탄환이 중복 생성되지 않도록 한다.
        _hasFired = true;

        if (_target == null ||
            !_target.isActiveAndEnabled ||
            _target.IsDead ||
            _firePos == null ||
            _bulletPrefab == null)
        {
            return;
        }

        int statBonus = _player.PlayerState.INT;

        int minDamage =
            Mathf.Max(1, _baseMinDamage + statBonus);

        int maxDamage =
            Mathf.Max(minDamage, _baseMaxDamage + statBonus);

        int damage = UnityEngine.Random.Range(minDamage, maxDamage + 1);

        Bullet bullet = Instantiate(
            _bulletPrefab,
            _firePos.position,
            Quaternion.identity);

        bullet.Initialize(_target, damage);
    }

    // 공격 종료 또는 취소 시 호출한다.
    public void FinishAttack()
    {
        IsAttacking = false;
        _hasFired = false;
        _target = null;
    }
}