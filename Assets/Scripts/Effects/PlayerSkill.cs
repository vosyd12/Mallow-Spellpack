using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerSkill : MonoBehaviour
{
    public enum UpgradeTarget
    {
        WaterBall,
        FireArea
    }

    private const float AutoCastCheckInterval = 0.1f;

    [SerializeField]
    private WaterBall _waterBall;

    [SerializeField]
    private FireAreaSkill _fireAreaSkill;

    [SerializeField]
    private EnemySpawner _enemySpawner;

    [SerializeField]
    private bool _autoWaterBall = true;

    [SerializeField]
    private bool _autoFireArea = true;

    private Player _player;

    private float _waterBallNextUseTime;
    private float _fireAreaNextUseTime;
    private float _nextAutoCastCheckTime;

    public float WaterBallCooldownDuration { get; private set; }
    public float FireAreaCooldownDuration { get; private set; }

    public float WaterBallCooldownRemaining =>
        Mathf.Max(0f, _waterBallNextUseTime - Time.time);

    public float FireAreaCooldownRemaining =>
        Mathf.Max(0f, _fireAreaNextUseTime - Time.time);

    public bool CanUseWaterBall =>
        _player != null &&
        _player.CanAct &&
        _waterBall != null &&
        _waterBall.isActiveAndEnabled &&
        WaterBallCooldownRemaining <= 0f;

    public bool CanManuallyUseWaterBall =>
        !_autoWaterBall && CanUseWaterBall;

    public bool CanUseFireArea =>
        _player != null &&
        _player.CanAct &&
        _fireAreaSkill != null &&
        _fireAreaSkill.isActiveAndEnabled &&
        FireAreaCooldownRemaining <= 0f;


    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if(Time.time < _nextAutoCastCheckTime)
        {
            return;
        }

        _nextAutoCastCheckTime = Time.time + AutoCastCheckInterval;
        
        if(_autoWaterBall)
        {
            TryUseWaterBall();
        }

        if (_autoFireArea)
        {
            UseFireArea();
        }
    }

    public void UseWaterBall()
    {
        if(!_autoWaterBall)
        {
            TryUseWaterBall();
        }
    }

    private void TryUseWaterBall()
    {
        if (!isActiveAndEnabled || !CanUseWaterBall || _enemySpawner == null)
        {
            return;
        }

        Enemy target = _enemySpawner.FindNearestEnemy(_player.transform.position);

        if (target == null || !_waterBall.TryCast(_player, target))
        {
            return;
        }

        WaterBallCooldownDuration = Mathf.Max(0f, _waterBall.Cooldown);
        _waterBallNextUseTime = Time.time + WaterBallCooldownDuration;
    }

    public void UseFireArea()
    {
        if (!CanUseFireArea)
        {
            return;
        }

        if (!_fireAreaSkill.TryCast(_player))
        {
            return;
        }

        FireAreaCooldownDuration =
            Mathf.Max(0f, _fireAreaSkill.Cooldown);

        _fireAreaNextUseTime =
            Time.time + FireAreaCooldownDuration;
    }

    // 카드 선택 가능 여부는 스킬의 강화 가능 레벨을 기준으로 판단한다.
    public bool CanUpgrade(UpgradeTarget target)
    {
        return target switch
        {
            UpgradeTarget.WaterBall => _waterBall != null && _waterBall.CanLevelUp,
            UpgradeTarget.FireArea => _fireAreaSkill != null && _fireAreaSkill.CanLevelUp,
            _ => false
        };
    }

    public bool TryUpgrade(UpgradeTarget target)
    {
        return target switch
        {
            UpgradeTarget.WaterBall => _waterBall != null && _waterBall.TryLevelUp(),
            UpgradeTarget.FireArea => _fireAreaSkill != null && _fireAreaSkill.TryLevelUp(),
            _ => false
        };
    }
}