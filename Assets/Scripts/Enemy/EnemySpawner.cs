using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] 
    private Enemy[] _enemyPrefabs;

    [SerializeField] 
    private GameObject _player;

    [SerializeField] 
    private Transform _stage;

    [SerializeField] 
    private Transform _enemyParent;

    [SerializeField]
    private Enemy _bossPrefab;

    [SerializeField] 
    private int _stageSpawnerEnemyCount;

    [SerializeField] 
    private float _playerSafeDistance = 1.5f;

    [SerializeField] 
    private Vector2 _stageSize = new Vector2(6f, 7.5f);

    [SerializeField]
    private Stage _stageController;

    private float _edgePadding = 0.3f;
    private readonly List<Enemy> _enemies = new List<Enemy>();
    private float _nextSpawnAttemptTime;

    public IReadOnlyList<Enemy> SpawnedEnemies =>
        _enemies;

    private void Update()
    {
        _enemies.RemoveAll(enemy => enemy == null);

        // 재시작과 보스전 중에는 일반 적을 생성하지 않는다.
        if (_stageController == null || 
            !_stageController.CanSpawnNormalEnemies)
        {
            return;
        }

        // 목표 수량보다 초과해서 생산 X
        if (_enemies.Count >= _stageSpawnerEnemyCount ||
            Time.time < _nextSpawnAttemptTime)
        {
            return;
        }

        // 현재부터 0.5초 후에 다시 생성 딜레이
        _nextSpawnAttemptTime = Time.time + 0.5f;
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

            enemy.Initialize(_player, _stageController);
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

    // 적 정리 함수
    public void ClearEnemies()
    {
        foreach(Enemy enemy in _enemies)
        {
            if(enemy == null)
            {
                continue;
            }

            enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject);
        }

        _enemies.Clear();
        _nextSpawnAttemptTime = 0f;
    }

    public bool TrySpawnBoss(out Enemy boss)
    {
        boss = null;

        if (_bossPrefab == null || _player == null ||
            _stage == null || _stageController == null)
        {
            return false;
        }

        Vector3 playerPosition = _player.transform.position;
        Vector3 localPlayer = _stage.InverseTransformPoint(playerPosition);
        Vector2 spawnRange = _stageSize * 0.25f;

        Vector3 spawnPosition = _stage.TransformPoint(new Vector3(
            localPlayer.x < 0f ? spawnRange.x : -spawnRange.x,
            localPlayer.y < 0f ? spawnRange.y : -spawnRange.y,
            0f));

        Vector2 offset = spawnPosition - playerPosition;
        if (offset.sqrMagnitude < _playerSafeDistance * _playerSafeDistance)
        {
            return false;
        }

        ClearEnemies();

        boss = Instantiate(_bossPrefab, spawnPosition, Quaternion.identity, _enemyParent);
        boss.Initialize(_player, _stageController);
        _enemies.Add(boss);
        return true;
    }
}