using UnityEngine;

public class PureTrailSkill : SkillBehaviour
{
    [SerializeField] private PureTrailMark _markPrefab;
    [SerializeField, Min(0.1f)] private float _activeDuration = 3f;
    [SerializeField, Min(0.1f)] private float _markSpacing = 0.8f;

    [SerializeField]
    private SkillData _data = new SkillData
    {
        _skillId = "PureTrail",
        _element = Element.Pure,
        _shape = SkillData.SkillShape.Area,
        _baseDamage = 12f,
        _coolDown = 7f
    };

    private Player _owner;
    private Rigidbody2D _body;
    private Vector2 _previousPosition;
    private float _distanceSinceMark;
    private float _activeUntil;
    private int _damage;

    public override SkillData Data => _data;

    public override bool TryCast(Player player, EnemySpawner enemySpawner)
    {
        if (!isActiveAndEnabled ||
            _owner != null ||
            player == null ||
            !player.CanAct ||
            player.IsLevelUpCardOpen ||
            !player.IsMoving ||
            _markPrefab == null ||
            _data == null ||
            !player.TryGetComponent<Rigidbody2D>(out var body))
        {
            return false;
        }

        int damage = _data.GetDamage();
        if (damage <= 0)
        {
            return false;
        }

        _owner = player;
        _body = body;
        _previousPosition = body.position;
        _distanceSinceMark = 0f;
        _activeUntil = Time.time + _activeDuration;
        _damage = damage;

        return true;
    }

    private void FixedUpdate()
    {
        if (_owner == null)
        {
            return;
        }

        if (!_owner.isActiveAndEnabled || !_owner.CanAct ||
            Time.time >= _activeUntil)
        {
            _owner = null;
            return;
        }

        Vector2 position = _body.position;
        Vector2 offset = position - _previousPosition;
        float distance = offset.magnitude;

        if (distance <= 0f)
        {
            return;
        }

        float spacing = Mathf.Max(0.1f, _markSpacing);
        float nextMarkDistance = spacing - _distanceSinceMark;
        Vector2 direction = offset / distance;

        // 한 물리 프레임에 간격을 여러 번 넘겨도 경로 위에 고르게 배치한다.
        while (nextMarkDistance <= distance)
        {
            Vector2 markPosition = _previousPosition + direction * nextMarkDistance;
            PureTrailMark mark = Instantiate(_markPrefab, markPosition, Quaternion.identity);
            mark.Initialize(_owner, _damage);

            nextMarkDistance += spacing;
        }

        _distanceSinceMark = Mathf.Max(0f, distance + spacing - nextMarkDistance);
        _previousPosition = position;
    }

    private void OnDisable()
    {
        _owner = null;
    }
}