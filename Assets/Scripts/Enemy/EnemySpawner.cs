using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemyPrefabs;
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _stage;
    [SerializeField] private Transform _enemyParent;
    [SerializeField] private int _stageSpawnerEnemyCount;

    [SerializeField] private float _playerSafeDistance = 1.5f;

    [SerializeField] private Vector2 _stageSize = new Vector2(6f, 7.5f);

    private float _edgePadding = 0.3f;
    private readonly List<Enemy> _enemies = new List<Enemy>();
    private float _nextSpawnAttemptTime;

    private void Update()
    {
        _enemies.RemoveAll(enemy => enemy == null);

        if(_enemies.Count > _stageSpawnerEnemyCount || 
            Time.time < _nextSpawnAttemptTime)
        {
            return;
        }

        _nextSpawnAttemptTime = Time.time - 0.5f;
        Spawn();
    }


    private void Spawn()
    {
        if (_enemyPrefabs == null
            || _enemyPrefabs.Length == 0)
        {
            return;
        }

        if (_player == null ||
            _stage == null)
        {
            return;
        }

        // 랜덤한 size하고 element를 추출한다.
        EnemySize size = (EnemySize)Random.Range(0, (int)EnemySize.Max);
        Element element = (Element)Random.Range(0, (int)Element.Max);

        Enemy prefab = FindEnemyPrefab(size, element);

        if (prefab == null)
        {
            Debug.LogWarning(
                  $"등록되지 않은 적 조합: {size} / {element}");
            return;
        }

        Vector2 halfSize =
            _stageSize * 0.5f - Vector2.one * _edgePadding;

        if (halfSize.x <= 0f || halfSize.y <= 0f)
        {
            return;
        }

        float safeDistanceSquared =
           _playerSafeDistance * _playerSafeDistance;

        for (int attempt = 0; attempt < 40; attempt++)
        {
            Vector2 localPosition = GetRandomEdgePosition(halfSize);

            Vector3 position = _stage.TransformPoint(
                new Vector3(localPosition.x, localPosition.y, 0f));

            Vector2 distance =
                (Vector2)position - (Vector2)_player.transform.position;

            if (distance.sqrMagnitude < safeDistanceSquared)
                continue;

            Enemy enemy = Instantiate(
                prefab,
                position,
                Quaternion.identity,
                _enemyParent);

            enemy.Initialize(_player);
            _enemies.Add(enemy);

            return;
        }
    }

    private Vector2 GetRandomEdgePosition(Vector2 halfSize)
    { 

        int edge = Random.Range(0, 4);

        switch (edge)
        {
            case 0: // 왼쪽
                return new Vector2(
                    -halfSize.x,
                    Random.Range(-halfSize.y, halfSize.y));

            case 1: // 오른쪽
                return new Vector2(
                    halfSize.x,
                    Random.Range(-halfSize.y, halfSize.y));

            case 2: // 아래쪽
                return new Vector2(
                    Random.Range(-halfSize.x, halfSize.x),
                    -halfSize.y);

            default: // 위쪽
                return new Vector2(
                    Random.Range(-halfSize.x, halfSize.x),
                    halfSize.y);
        }
    }

    // 플레이어 기준 가장 가까운 적을 찾는 함수
    public Enemy FindNearestEnemy(Vector2 position)
    {
        Enemy nearest = null;
        // 
        float nearestDistance = float.MaxValue;

        foreach (Enemy enemy in _enemies)
        {
            if (enemy == null || 
                !enemy.isActiveAndEnabled ||
                enemy.IsDead)
            {
                continue;
            }

            float distance =
                ((Vector2)enemy.transform.position - position).sqrMagnitude;

            if (distance >= nearestDistance)
            {
                continue;
            }

            nearestDistance = distance;
            nearest = enemy;
        }

        return nearest;
    }

    // 매개변수로 받은 size하고 element를 가지고 생성하는 함수
    private Enemy FindEnemyPrefab(
        EnemySize size, 
        Element element)
    {
        foreach (Enemy prefab in _enemyPrefabs)
        {
            if (prefab == null)
            {
                continue;
            }

            if (prefab.EnemyState.Size == size &&
                prefab.EnemyState.Element == element)
            {
                return prefab;
            }
        }

        return null;
    }
}