using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemyPrefabs;
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _stage;
    [SerializeField] private Transform _enemyParent;

    [SerializeField] private int _spawnCount = 10;
    [SerializeField] private float _playerSafeDistance = 1.5f;

    [SerializeField] private Vector2 _stageSize = new Vector2(6f, 7.5f);
    [SerializeField] private float _edgePadding = 0.3f;

    private readonly List<Enemy> _enemies = new List<Enemy>();

    private void Start()
    {
        for(int i  = 0; i < _spawnCount; i++)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (_enemyPrefabs == null || _enemyPrefabs.Length == 0) return;

        int index = Random.Range(0, _enemyPrefabs.Length);
        Enemy Prefab = _enemyPrefabs[index];

        if (Prefab == null)
        {
            Debug.LogWarning("Enemy Prefab이 연결되지 않았습니다.");
            return;
        }

        Vector2 halfSize = _stageSize * 0.5f;
        float safeDistanceSquared = _playerSafeDistance * _playerSafeDistance;

        // 플레이어와 겹치면 다른 위치를 시도한다
        for(int attempt = 0; attempt < 20; attempt++)
        {
            float x = Random.Range(
                      -halfSize.x + _edgePadding,
                      halfSize.x - _edgePadding);

            float y = Random.Range(
                -halfSize.y + _edgePadding,
                halfSize.y - _edgePadding);

            Vector3 position =
                _stage.TransformPoint(new Vector3(x, y, 0f));

            Vector2 distance = position - _player.transform.position;

            if (distance.sqrMagnitude < safeDistanceSquared)
                continue;

            Enemy enemy = Instantiate(
                    Prefab,
                    position,
                    Quaternion.identity,
                    _enemyParent);

            enemy.Initialize(_player);
            _enemies.Add(enemy);

            return;
        }
    }

    public Enemy FindNearestEnemy(Vector2 position)
    {
        Enemy nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Enemy enemy in _enemies)
        {
            if (enemy == null || !enemy.isActiveAndEnabled)
                continue;

            float distance =
                ((Vector2)enemy.transform.position - position).sqrMagnitude;

            if (distance >= nearestDistance)
                continue;

            nearestDistance = distance;
            nearest = enemy;
        }

        return nearest;
    }
}