
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerSkill : MonoBehaviour
{
    private const float AutoCastCheckInterval = 0.1f;

    // 카드에 등장할 수 있는 스킬을 Inspector에서 등록한다.
    [SerializeField] 
    private SkillBehaviour[] _skills = Array.Empty<SkillBehaviour>();

    [SerializeField] 
    private EnemySpawner _enemySpawner;

    private Player _player;
    private float _nextAutoCastCheckTime;

    private readonly List<SkillBehaviour> _learnedSkills = new();

    // Player.TryLevelUp()이 사용한다. 첫 획득이 가능한 카드도 포함한다.
    public bool HasAvailableUpgrade =>
        Array.Exists(_skills, CanOfferCard);

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!_player.CanAct ||
            _player.IsLevelUpCardOpen ||
            Time.time < _nextAutoCastCheckTime)
        {
            return;
        }

        _nextAutoCastCheckTime = Time.time + AutoCastCheckInterval;

        foreach (SkillBehaviour skill in _skills)
        {
            if (skill != null)
            {
                // 획득 여부와 쿨타임은 각 스킬이 확인한다.
                skill.TryAutoCast(_player, _enemySpawner);
            }
        }
    }

    public void GetCardCandidates(List<SkillBehaviour> candidates)
    {
        candidates.Clear();

        foreach (SkillBehaviour skill in _skills)
        {
            if (CanOfferCard(skill))
            {
                candidates.Add(skill);
            }
        }
    }

    private static bool CanOfferCard(SkillBehaviour skill)
    {
        return skill != null && skill.isActiveAndEnabled && skill.CanChooseCard;
    }

    public SkillBehaviour GetLearnedSkill(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < _learnedSkills.Count
            ? _learnedSkills[slotIndex]
            : null;
    }

    public bool TryChooseSkill(SkillBehaviour skill)
    {
        if (!_player.IsLevelUpCardOpen ||
            !CanOfferCard(skill) ||
            Array.IndexOf(_skills, skill) < 0)
        {
            return false;
        }

        bool isFirstChoice = !skill.IsLearned;

        if (!skill.TryChooseCard())
        {
            return false;
        }

        // 강화한 스킬은 기존 슬롯에 유지한다.
        if (isFirstChoice)
        {
            _learnedSkills.Add(skill);
        }

        return true;
    }
}