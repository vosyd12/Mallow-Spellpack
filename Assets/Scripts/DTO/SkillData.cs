using System;
using UnityEngine;


[Serializable]
public class SkillData
{
    public enum SkillShape
    {
        Projectile,
        Area,
        Chain,
        Orbit
    }

    public string _skillId;
    public Element _element;
    public SkillShape _shape;

    [Min(1)]
    public int _level = 1;

    [Min(1)]
    public int _maxLevel = 5;

    [Min(0f)]
    public float _baseDamage;

    [Min(0f)]
    public float _damagePerLevel = 0.2f;

    [Min(0f)]
    public float _coolDown;

    public float GetDamageMultiplier()
    {
        int maxLevel = Mathf.Max(1, _maxLevel);
        int level = Mathf.Clamp(_level, 1, maxLevel);

        return 1f +
            (level - 1) * Mathf.Max(0f, _damagePerLevel);
    }

    public int GetDamage()
    {
        float baseDamage = Mathf.Max(0f, _baseDamage);

        return Mathf.RoundToInt(
            baseDamage * GetDamageMultiplier());
    }
}