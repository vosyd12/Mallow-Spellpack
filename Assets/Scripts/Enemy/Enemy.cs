using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _traceObject;
    [SerializeField] private bool _useDefaultMovement = true;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private HealthBar _healthBar;

    [SerializeField] private int _maxHP;

    [SerializeField]
    private HitReaction _hitReaction;

    [SerializeField]
    private EnemyState _enemy;
    public EnemyState EnemyState => _enemy;

    public Transform Target => _traceObject != null ? _traceObject.transform : null;
    public float MoveSpeed => _enemy.MoveSpeed;

    private Rigidbody2D _rd;

    private Vector3 _pos;

    private void Awake()
    {
        _enemy.HP = _maxHP;
        _enemy.MP = 50;
        _enemy.Attack = 5;
        _enemy.Defense = 1;
        _enemy.Exp = 10;
        _enemy.MoveSpeed = _moveSpeed;
        _enemy.AttackSpeed = 5f;

        _rd = GetComponent<Rigidbody2D>();
        _healthBar.SetHealth(_enemy.HP, _maxHP);
    }

    private void Update()
    {
        // 추적대상이 없을 경우 가만히
        if(_traceObject == null)
        {
            _pos = Vector3.zero;
            return;
        }

        // 캐릭터와 거리를 확인
        _pos = (_traceObject.transform.position - _rd.transform.position).normalized;
    }

    private void FixedUpdate()
    {
        if (!_useDefaultMovement) return;

        Vector2 nextPosition = _rd.position + (Vector2)_pos * _enemy.MoveSpeed * Time.fixedDeltaTime;

        // 캐릭터를 향해서 이동
        _rd.MovePosition(nextPosition);
    }

    public void TakeDamage(int damage)
    {
        if(_enemy.HP <= 0 || damage <= 0)
        {
            return;
        }

        _enemy.HP = Mathf.Max(0, _enemy.HP - damage);

        _healthBar.SetHealth(_enemy.HP, _maxHP);
        _hitReaction.Play();

        if (_enemy.HP > 0)
        {
            return;
        }

        if(_traceObject != null &&
           _traceObject.TryGetComponent<Player>(out var player))
        {
            player.GainExperience(_enemy.Exp);
        }

        Destroy(gameObject);
    }

    public void Initialize(GameObject player)
    {
        _traceObject = player;
    }
}
