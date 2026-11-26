using System.Collections.Generic;
using UnityEngine;

public class PureTrailMark : MonoBehaviour
{
    [SerializeField] private GameObject _impactPrefab;
    [SerializeField, Min(0.1f)] private float _radius = 0.5f;
    [SerializeField, Min(0.1f)] private float _lifetime = 2f;
    [SerializeField] private LayerMask _enemyLayers = ~0;

    private readonly List<Collider2D> _hits = new List<Collider2D>(8);
    private readonly HashSet<Enemy> _targets = new HashSet<Enemy>();

    private ContactFilter2D _filter;
    private Player _owner;
    private float _expiresAt;
    private int _damage;

    public void Initialize(Player owner, int damage)
    {
        _owner = owner;
        _damage = damage;
        _expiresAt = Time.time + _lifetime;

        _filter = new ContactFilter2D { useTriggers = true };
        _filter.SetLayerMask(_enemyLayers);
    }

    private void FixedUpdate()
    {
        if (_owner == null || !_owner.isActiveAndEnabled ||
            !_owner.CanAct || Time.time >= _expiresAt)
        {
            Destroy(gameObject);
            return;
        }

        _hits.Clear();
        Physics2D.OverlapCircle(transform.position, _radius, _filter, _hits);

        foreach (Collider2D hit in _hits)
        {
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null && enemy.isActiveAndEnabled && enemy.CanAct)
            {
                _targets.Add(enemy);
            }
        }

        if (_targets.Count == 0)
        {
            return;
        }

        // Destroy가 처리되기 전에도 폭발 판정이 다시 실행되지 않게 한다.
        enabled = false;

        foreach (Enemy enemy in _targets)
        {
            enemy.TakeDamage(_damage);
        }

        if (_impactPrefab != null)
        {
            Instantiate(_impactPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}