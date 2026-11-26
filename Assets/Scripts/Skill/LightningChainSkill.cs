using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningChainSkill : SkillBehaviour
{
    private const float ChainEffectInterval = 0.055f;

    [SerializeField]
    private GameObject _arcPrefab;

    [SerializeField]
    private GameObject _impactPrefab;

    [SerializeField, Min(0.1f)]
    private float _castRange = 6f;

    [SerializeField, Min(0.1f)]
    private float _chainRange = 2.5f;

    [SerializeField, Min(1)]
    private int _maxTargets = 3;

    [SerializeField]
    private SkillData _data = new SkillData
    {
        _skillId = "LightningChain",
        _element = Element.Lightning,
        _shape = SkillData.SkillShape.Chain,
        _baseDamage = 15f,
        _coolDown = 4f
    };

    private readonly HashSet<Enemy> _hitEnemies = new HashSet<Enemy>();

    public override SkillData Data => _data;

    public override bool TryCast(Player player, EnemySpawner enemySpawner)
    {
        if (!isActiveAndEnabled ||
            player == null ||
            !player.CanAct ||
            player.IsLevelUpCardOpen ||
            enemySpawner == null ||
            _data == null)
        {
            return false;
        }

        int damage = _data.GetDamage();
        if (damage <= 0)
        {
            return false;
        }

        _hitEnemies.Clear();

        Vector2 origin = player.transform.position;
        float range = _castRange;
        bool playEffects = _arcPrefab != null || _impactPrefab != null;
        List<(Vector2 From, Vector2 To)> effectLinks = null;

        for (int i = 0; i < _maxTargets; i++)
        {
            Enemy target = FindNextTarget(enemySpawner.SpawnedEnemies, origin, range);
            if (target == null)
            {
                break;
            }

            _hitEnemies.Add(target);

            // 대상이 이번 공격으로 죽어도 타격 위치에서 다음 연쇄를 시작한다.
            Vector2 hitPosition = target.transform.position;
            if (playEffects)
            {
                effectLinks ??= new List<(Vector2, Vector2)>(_maxTargets);
                effectLinks.Add((origin, hitPosition));
            }

            origin = hitPosition;
            range = _chainRange;

            target.TakeDamage(damage);
        }

        // 첫 대상이 없으면 쿨다운을 시작할 필요가 없다.
        if (effectLinks != null)
        {
            StartCoroutine(PlayChainEffects(effectLinks));
        }

        return _hitEnemies.Count > 0;
    }

    private IEnumerator PlayChainEffects(List<(Vector2 From, Vector2 To)> links)
    {
        for (int i = 0; i < links.Count; i++)
        {
            if (i > 0)
            {
                yield return new WaitForSeconds(ChainEffectInterval);
            }

            var (from, to) = links[i];
            Vector2 direction = to - from;
            if (_arcPrefab != null && direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                GameObject arc = Instantiate(_arcPrefab, from, Quaternion.Euler(0f, 0f, angle));
                Vector3 scale = arc.transform.localScale;
                scale.x *= direction.magnitude;
                arc.transform.localScale = scale;
            }

            if (_impactPrefab != null)
            {
                Instantiate(_impactPrefab, to, Quaternion.identity);
            }
        }
    }

    private Enemy FindNextTarget(
        IReadOnlyList<Enemy> enemies,
        Vector2 origin,
        float range)
    {
        Enemy nearest = null;
        float nearestDistanceSquared = range * range;

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null ||
                !enemy.isActiveAndEnabled ||
                !enemy.CanAct ||
                _hitEnemies.Contains(enemy))
            {
                continue;
            }

            Vector2 offset = (Vector2)enemy.transform.position - origin;
            float distanceSquared = offset.sqrMagnitude;

            if (distanceSquared > nearestDistanceSquared)
            {
                continue;
            }

            nearest = enemy;
            nearestDistanceSquared = distanceSquared;
        }

        return nearest;
    }
}