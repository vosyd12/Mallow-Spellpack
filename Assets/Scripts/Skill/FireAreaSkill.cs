using UnityEngine;

public class FireAreaSkill : SkillBehaviour
{
    [SerializeField]
    private FireArea _areaPrefab;

    [SerializeField, Min(0f)]
    private float _range = 6f;

    [SerializeField, Min(0.1f)]
    private float _radius = 1.5f;

    [SerializeField, Min(0.1f)]
    private float _duration = 3f;

    [SerializeField, Min(0.05f)]
    private float _damageInterval = 0.5f;

    [SerializeField]
    private SkillData _data = new SkillData
    {
        _skillId = "FireArea",
        _element = Element.Fire,
        _shape = SkillData.SkillShape.Area,
        _baseDamage = 5f,
        _coolDown = 5f
    };

    public override SkillData Data => _data;

    public override bool TryCast(Player player, EnemySpawner enemySpawner)
    {
        if (!isActiveAndEnabled ||
            player == null ||
            !player.CanAct ||
            _areaPrefab == null ||
            _data == null)
        {
            return false;
        }

        Enemy target = player.Target;

        if (target == null ||
            !target.isActiveAndEnabled ||
            target.IsDead)
        {
            return false;
        }

        Vector2 offset =
            target.transform.position - player.transform.position;

        float range = Mathf.Max(0f, _range);

        if (offset.sqrMagnitude > range * range)
        {
            return false;
        }

        int damage = _data.GetDamage();

        if (damage <= 0)
        {
            return false;
        }

        FireArea area = Instantiate(
            _areaPrefab,
            target.transform.position,
            Quaternion.identity);

        area.Initialize(
            player,
            damage,
            Mathf.Max(0.1f, _radius),
            Mathf.Max(0.1f, _duration),
            Mathf.Max(0.05f, _damageInterval));

        return true;
    }
}