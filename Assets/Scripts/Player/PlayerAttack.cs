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
    private int _baseMinDamage = 5;
    private int _baseMaxDamage = 10;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _attackTimer -= Time.deltaTime;

        _target = _player.Target;

        if (_target == null || 
            !_target.isActiveAndEnabled)
        {
            return;
        }

        if (_attackTimer > 0f)
        {
            return;
        }

        Vector2 direction = 
            _target.transform.position - transform.position;

        if (direction.sqrMagnitude > _attackRange * _attackRange)
        {
            return;
        }

        if(_player.Job == EPlayerJob.EWizard)
        {
            MagicAttack();
        }

        _attackTimer = _attackInterval;
    }

    private void MagicAttack()
    {
        if(_firePos == null)
        {
            return;
        }

        int statBonus = _player.PlayerState.INT;

        int minDamage = Mathf.Max(1, _baseMinDamage + statBonus);
        int maxDamage = Mathf.Max(minDamage, _baseMaxDamage + statBonus);

        int damage = Random.Range(minDamage, maxDamage + 1);

        Bullet bullet = Instantiate(
            _bulletPrefab, _firePos.position, Quaternion.identity);

        bullet.Initialize(_target, damage);
    }
}