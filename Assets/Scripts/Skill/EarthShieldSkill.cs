using UnityEngine;

public class EarthShieldSkill : SkillBehaviour
{
    [SerializeField, Min(0.1f)]
    private float _baseDuration = 2f;

    [SerializeField, Min(0f)]
    private float _durationPerLevel = 0.5f;

    [SerializeField]
    private SkillData _data = new SkillData
    {
        _skillId = "EarthShield",
        _element = Element.Earth,
        _shape = SkillData.SkillShape.Shield,
        _coolDown = 8f
    };

    public override SkillData Data => _data;

    public override bool TryCast(Player player, EnemySpawner enemySpawner)
    {
        if (!isActiveAndEnabled || player == null || _data == null)
        {
            return false;
        }

        // 강화하면 보호막의 유지 시간만 늘어난다.
        int maxLevel = Mathf.Max(1, _data._maxLevel);
        int level = Mathf.Clamp(_data._level, 1, maxLevel);
        float duration = _baseDuration + (level - 1) * _durationPerLevel;

        return player.TryGrantShield(duration);
    }
}