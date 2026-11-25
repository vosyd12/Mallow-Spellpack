using UnityEngine;

public abstract class SkillBehaviour : MonoBehaviour
{
    public abstract SkillData Data { get; }
    public abstract bool TryCast(Player player, EnemySpawner enemySpawner);
}