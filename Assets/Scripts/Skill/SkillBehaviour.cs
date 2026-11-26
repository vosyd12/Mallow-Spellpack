using UnityEngine;

public abstract class SkillBehaviour : MonoBehaviour
{
    private float _nextCastTime;

    public abstract SkillData Data { get; }

    public bool IsLearned { get; private set; }
    public float CooldownDuration { get; private set; }
    public float CooldownRemaining => Mathf.Max(0f, _nextCastTime - Time.time);

    // 아직 배우지 않았거나, 배운 뒤 강화할 수 있으면 카드 후보가 된다.
    public bool CanChooseCard =>
        isActiveAndEnabled &&
        Data != null &&
        (!IsLearned || Data.CanLevelUp);

    public bool TryChooseCard()
    {
        if (!CanChooseCard)
        {
            return false;
        }

        if (IsLearned)
        {
            return Data.TryLevelUp();
        }

        // 첫 선택에서 Lv.1로 획득한다.
        Data._level = 1;
        IsLearned = true;
        return true;
    }

    public bool TryAutoCast(Player player, EnemySpawner enemySpawner)
    {
        if (!isActiveAndEnabled ||
            !IsLearned ||
            Data == null ||
            CooldownRemaining > 0f)
        {
            return false;
        }

        // 대상이 없거나 이동 조건을 충족하지 못하면 쿨타임을 소비하지 않는다.
        if (!TryCast(player, enemySpawner))
        {
            return false;
        }

        CooldownDuration = Mathf.Max(0f, Data._coolDown);
        _nextCastTime = Time.time + CooldownDuration;
        return true;
    }

    public abstract bool TryCast(Player player, EnemySpawner enemySpawner);
}