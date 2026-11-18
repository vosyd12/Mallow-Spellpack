using System.Collections.Generic;
using UnityEngine;

public class FireArea : MonoBehaviour
{
    [SerializeField]
    private LayerMask _enemyLayers = ~0;

    private readonly List<Collider2D> _hits =
        new List<Collider2D>(16);

    private readonly HashSet<Enemy> _damagedEnemies =
        new HashSet<Enemy>();

    private ContactFilter2D _filter;

    private Player _owner;
    private int _damage;
    private float _radius;
    private float _damageInterval;
    private float _expireTime;
    private float _nextDamageTime;
    private bool _isInitialized;

    public void Initialize(
        Player owner,
        int damage,
        float radius,
        float duration,
        float damageInterval)
    {
        if (owner == null || !owner.CanAct || damage <= 0)
        {
            Destroy(gameObject);
            return;
        }

        _owner = owner;
        _damage = damage;
        _radius = Mathf.Max(0.1f, radius);
        _damageInterval = Mathf.Max(0.05f, damageInterval);

        _expireTime =
            Time.time + Mathf.Max(0.1f, duration);

        _nextDamageTime = Time.time;

        _filter = new ContactFilter2D
        {
            useTriggers = true
        };

        _filter.SetLayerMask(_enemyLayers);

        _isInitialized = true;
    }

    private void FixedUpdate()
    {
        if (!_isInitialized)
        {
            return;
        }

        if (_owner == null ||
            !_owner.isActiveAndEnabled ||
            !_owner.CanAct ||
            Time.time >= _expireTime)
        {
            _isInitialized = false;
            Destroy(gameObject);
            return;
        }

        if (Time.time < _nextDamageTime)
        {
            return;
        }

        _nextDamageTime =
            Time.time + _damageInterval;

        ApplyDamage();
    }

    private void ApplyDamage()
    {
        _hits.Clear();
        _damagedEnemies.Clear();

        Physics2D.OverlapCircle(
            (Vector2)transform.position,
            _radius,
            _filter,
            _hits);

        foreach (Collider2D hit in _hits)
        {
            if (hit == null)
            {
                continue;
            }

            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy == null ||
                !enemy.isActiveAndEnabled ||
                !enemy.CanAct ||
                !_damagedEnemies.Add(enemy))
            {
                continue;
            }

            enemy.TakeDamage(_damage);
        }
    }
}