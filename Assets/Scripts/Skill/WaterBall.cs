using UnityEngine;

public class WaterBall : SkillBehaviour
{
    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private Transform _firePos;

    [SerializeField, Min(0f)]
    private float _range = 6f;

    [SerializeField]
    private SkillData _data = new SkillData
    {
        _skillId = "Waterball",
        _element = Element.Water,
        _shape = SkillData.SkillShape.Projectile,
        _baseDamage = 20f,
        _coolDown = 3f
    };

    public override SkillData Data => _data;

    public override bool TryCast(Player player, EnemySpawner enemySpawner)
    {
        if (!isActiveAndEnabled ||
            player == null ||
            !player.CanAct ||
            enemySpawner == null ||
            _bulletPrefab == null ||
            _firePos == null ||
            _data == null)
        {
            return false;
        }

        // 기존 자동 발동과 동일하게 가장 가까운 적을 찾는다.
        Enemy target = enemySpawner.FindNearestEnemy(player.transform.position);
        if (target == null)
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

        Bullet bullet = Instantiate(
            _bulletPrefab,
            _firePos.position,
            Quaternion.identity);

        bullet.Initialize(target, damage);

        return true;
    }
}