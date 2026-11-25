using System.Collections.Generic;
using UnityEngine;

public class LightningChainSkill : SkillBehaviour
{
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

        for (int i = 0; i < _maxTargets; i++)
        {
            Enemy target = FindNextTarget(enemySpawner.SpawnedEnemies, origin, range);
            if (target == null)
            {
                break;
            }

            _hitEnemies.Add(target);

            // 대상이 이번 공격으로 죽어도 타격 위치에서 다음 연쇄를 시작한다.
            origin = target.transform.position;
            range = _chainRange;

            target.TakeDamage(damage);
        }

        // 첫 대상이 없으면 쿨다운을 시작할 필요가 없다.
        return _hitEnemies.Count > 0;
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