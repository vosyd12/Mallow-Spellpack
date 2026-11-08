using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageScenery : MonoBehaviour
{
    [SerializeField]
    private Transform _sceneryRoot;

    [Serializable]
    private class SceneryGroup
    {
        public string Name;
        public GameObject[] Prefabs;
        public Transform[] SpawnPoints;
        public Vector2Int CountRange = new Vector2Int(3, 4);
        public Vector2 ScaleRange = new Vector2(0.9f, 1.1f);
    }

    [SerializeField]
    private SceneryGroup[] _groups;

    [Header("Variation")]
    [SerializeField, Min(0f)]
    private float _positionJitter = 0.15f;

    [SerializeField, Min(0f)]
    private float _minimumSpacing = 0.65f;

    private readonly List<GameObject> _spawnedObjects = new List<GameObject>();
    private readonly List<Vector3> _placedPositions = new List<Vector3>();

    public void Rebuild()
    {
        if (_sceneryRoot == null || _groups == null || _groups.Length == 0)
        {
            Debug.LogError("배경 부모와 타입별 배치 설정을 연결해주세요.", this);
            return;
        }

        ClearSpawnedObjects();

        foreach (SceneryGroup group in _groups)
        {
            if (group == null)
            {
                continue;
            }

            int minimum = Mathf.Max(0, group.CountRange.x);
            int maximum = Mathf.Max(minimum, group.CountRange.y);
            SpawnGroup(group, Random.Range(minimum, maximum + 1));
        }
    }

    private void ClearSpawnedObjects()
    {
        foreach (GameObject scenery in _spawnedObjects)
        {
            if (scenery == null)
            {
                continue;
            }

            // Destroy가 프레임 끝에 처리되기 전에도 이전 장식은 보이지 않게 한다.
            scenery.SetActive(false);
            Destroy(scenery);
        }

        _spawnedObjects.Clear();
        _placedPositions.Clear();
    }

    private void SpawnGroup(SceneryGroup group, int count)
    {
        if (count <= 0)
        {
            return;
        }

        var availablePrefabs = new List<GameObject>();
        var availablePoints = new List<Transform>();

        if (group.Prefabs != null)
        {
            foreach (GameObject prefab in group.Prefabs)
            {
                if (prefab != null)
                {
                    availablePrefabs.Add(prefab);
                }
            }
        }

        if (group.SpawnPoints != null)
        {
            foreach (Transform point in group.SpawnPoints)
            {
                if (point != null && !availablePoints.Contains(point))
                {
                    availablePoints.Add(point);
                }
            }
        }

        if (availablePrefabs.Count == 0 || availablePoints.Count == 0)
        {
            Debug.LogWarning($"{group.Name}: Prefab과 배치 지점을 연결해주세요.", this);
            return;
        }

        float minimumScale = Mathf.Max(0.01f, Mathf.Min(group.ScaleRange.x, group.ScaleRange.y));
        float maximumScale = Mathf.Max(minimumScale, Mathf.Max(group.ScaleRange.x, group.ScaleRange.y));
        int placedCount = 0;

        while (placedCount < count && availablePoints.Count > 0)
        {
            int pointIndex = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[pointIndex];
            availablePoints.RemoveAt(pointIndex);

            Vector2 offset = Random.insideUnitCircle * Mathf.Max(0f, _positionJitter);
            Vector3 position = point.position + new Vector3(offset.x, offset.y, 0f);

            if (!HasSpace(position))
            {
                continue;
            }

            GameObject prefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            GameObject scenery = Instantiate(prefab, position, Quaternion.identity, _sceneryRoot);
            scenery.transform.localScale = prefab.transform.localScale * Random.Range(minimumScale, maximumScale);

            _spawnedObjects.Add(scenery);
            _placedPositions.Add(position);
            placedCount++;
        }

        if (placedCount < count)
        {
            Debug.LogWarning($"{group.Name}: 배치 공간이 부족해 {placedCount}/{count}개를 배치했습니다.", this);
        }
    }

    private bool HasSpace(Vector3 position)
    {
        float minimumDistanceSquared = _minimumSpacing * _minimumSpacing;

        foreach (Vector3 placedPosition in _placedPositions)
        {
            if ((position - placedPosition).sqrMagnitude < minimumDistanceSquared)
            {
                return false;
            }
        }

        return true;
    }
}
