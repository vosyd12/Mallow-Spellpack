using System;
using UnityEngine;

public class SkillDatta : MonoBehaviour
{
    public enum SkillShape
    {
        Projectile,
        Area,
        Chain,
        Orbit
    }

    [Serializable]
    public class SkillData
    {
        public string _SkillID;

        public Element _element;
        public SkillShape _shape;

        public int _level = 1;
        public int _maxLevel = 5;

        public float _baseDamage;
        public float _damagePerLevel;
        public float _coolDown;

        public float GetDamageMultiplier()
        {
            return 1f + (_level - 1) * _damagePerLevel;
        }
    }
}