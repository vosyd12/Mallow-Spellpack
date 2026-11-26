using UnityEngine;

public class WindPulseSkill : SkillBehaviour
{
    private const float EffectPrefabRadius = 2f;

    [SerializeField]
    private GameObject _pulsePrefab;

    [SerializeField, Min(0.1f)]
    private float _radius = 2f;

    [SerializeField, Min(0f)]
    private float _radiusPerLevel = 0.2f;

    [SerializeField, Min(0.1f)]
    private float _knockbackSpeed = 8f;

    [SerializeField, Min(0.01f)]
    private float _knockbackDuration = 0.18f;

    [SerializeField]
    private SkillData _data = new SkillData
    {
        _skillId = "WindPulse",
        _element = Element.Wind,
        _shape = SkillData.SkillShape.Area,
        _coolDown = 6f
    };

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

        int level = Mathf.Clamp(_data._level, 1, Mathf.Max(1, _data._maxLevel));
        float radius = _radius + (level - 1) * _radiusPerLevel;
        float radiusSquared = radius * radius;
        Vector2 origin = player.transform.position;
        bool pushedAnyEnemy = false;

        foreach (Enemy enemy in enemySpawner.SpawnedEnemies)
        {
            if (enemy == null ||
                !enemy.isActiveAndEnabled ||
                !enemy.CanAct ||
                enemy.EnemyState.IsBoss)
            {
                continue;
            }

            Vector2 offset = (Vector2)enemy.transform.position - origin;
            if (offset.sqrMagnitude > radiusSquared ||
                !enemy.TryGetComponent<SlimeMotion>(out var motion))
            {
                continue;
            }

            // 중심이 겹친 적도 밀려날 수 있도록 방향을 정한다.
            Vector2 direction = offset.sqrMagnitude > 0.0001f
                ? offset.normalized
                : Vector2.right;

            if (motion.TryKnockback(direction, _knockbackSpeed, _knockbackDuration))
            {
                pushedAnyEnemy = true;
            }
        }

        if (pushedAnyEnemy && _pulsePrefab != null)
        {
            GameObject pulse = Instantiate(_pulsePrefab, origin, Quaternion.identity);
            pulse.transform.localScale *= radius / EffectPrefabRadius;
        }

        return pushedAnyEnemy;
    }
}