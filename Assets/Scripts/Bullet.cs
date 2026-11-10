using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _lifeTime = 5f;

    private Enemy _target;
    private Rigidbody2D _rd;
    private int _damage;
    private bool _hasHit;

    private void Awake()
    {
        _rd = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Enemy target, int damage)
    {
        _target = target;
        _damage = damage;

        Destroy(gameObject, _lifeTime);
    }

    private void FixedUpdate()
    {
        if (_hasHit) return;

        if(_target == null || 
            !_target.isActiveAndEnabled ||
            _target.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction = (Vector2)_target.transform.position - _rd.position;

        _rd.linearVelocity = direction.normalized * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit || _target == null) return;

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != _target) return;

        _hasHit = true;
        _rd.linearVelocity = Vector2.zero;

        enemy.TakeDamage(_damage);
        Destroy(gameObject);
    }
}