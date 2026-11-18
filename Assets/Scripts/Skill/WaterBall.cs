using UnityEngine;

public class WaterBall : MonoBehaviour
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

    public float Cooldown =>
        _data != null
            ? Mathf.Max(0f, _data._coolDown)
            : 0f;

    public bool TryCast(Player player, Enemy target)
    {
        if (!isActiveAndEnabled ||
            player == null ||
            !player.CanAct ||
            _bulletPrefab == null ||
            _firePos == null ||
            _data == null)
        {
            return false;
        }

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

        Bullet bullet = Instantiate(
            _bulletPrefab,
            _firePos.position,
            Quaternion.identity);

        bullet.Initialize(target, damage);

        return true;
    }
}