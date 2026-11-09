using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageScenery : MonoBehaviour
{
    [Serializable]
    private class SceneryGroup
    {
        public GameObject[] Prefabs;                // 생성할 오브젝트 후보
        public Transform[] SpawnPoints;             // 생성할 오브젝트 위치
        public Vector2 ScaleRange = new Vector2(0.9f, 1.1f);    // Prefab원래 크기에 곱할 최소. 최대 배율 사이즈
    }

    [SerializeField]
    private Transform _sceneryRoot;

    [SerializeField, Header("나무")]
    private SceneryGroup _trees;

    // x는 최소 값, y는 최대 값이다.
    // 같은 값을 넣으면 고정 수량으로 배치된다.
    [SerializeField]
    private Vector2Int _treeCountRange = new Vector2Int(3, 4);

    [SerializeField, Header("풀숲")]
    private SceneryGroup _bushes;

    [SerializeField]
    private Vector2Int _bushCountRange = new Vector2Int(3, 4);

    [SerializeField, Header("꽃풀숲")]
    private SceneryGroup _flowerBushes;

    [SerializeField]
    private Vector2Int _flowerBushCountRange = new Vector2Int(3, 4);

    [SerializeField, Header("꽃")]
    private SceneryGroup _flowers;

    [SerializeField]
    private Vector2Int _flowerCountRange = new Vector2Int(3, 4);

    [SerializeField, Min(0f), Header("배치 변화")]
    private float _positionJitter = 0.15f;

    [SerializeField, Min(0f)]
    private float _minimumSpacing = 0.65f;

    private readonly List<GameObject> _spawnedObjects = new List<GameObject>();

    private readonly List<Vector3> _placedPositions = new List<Vector3>();

    // 이전 배경을 지우고 타입별로 다시 배치한다.
    public void Rebuild()
    {
        if (_sceneryRoot == null)
        {
            Debug.LogError("배경 부모를 연결해주세요.", this);
            return;
        }

        ClearSpawnedObjects();

        SpawnGroup(_trees, _treeCountRange, "나무");
        SpawnGroup(_bushes, _bushCountRange, "풀숲");
        SpawnGroup(_flowerBushes, _flowerBushCountRange, "꽃풀숲");
        SpawnGroup(_flowers, _flowerCountRange, "꽃");
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

    private void SpawnGroup(
        SceneryGroup group, 
        Vector2Int countRange,
        string groupName)
    {
        int minimumCount = Mathf.Max(0, countRange.x);
        int maximumCount = Mathf.Max(minimumCount, countRange.y);
        int count = Random.Range(minimumCount, maximumCount + 1);

        if(count == 0)
        {
            return;
        }

        if(group == null)
        {
            Debug.LogWarning($"{groupName}: 배치 설정이 없습니다.", this);
            return;
        }

        List<GameObject> prefabs = GetAvailablePrefabs(group.Prefabs);
        List<Transform> points = GetAvailablePoints(group.SpawnPoints);

        if (prefabs.Count == 0 || points.Count == 0)
        {
            Debug.LogWarning(
                $"{groupName}: Prefab과 배치 지점을 연결해주세요.",
                this);
            return;
        }

        float minimumScale = Mathf.Max(
            0.01f,
            Mathf.Min(group.ScaleRange.x, group.ScaleRange.y));

        float maximumScale = Mathf.Max(
            minimumScale,
            Mathf.Max(group.ScaleRange.x, group.ScaleRange.y));

        int placedCount = 0;

        while (placedCount < count)
        {
            if (!TryGetPosition(points, out Vector3 position))
            {
                break;
            }

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

            GameObject scenery = Instantiate(
                prefab,
                position,
                Quaternion.identity,
                _sceneryRoot);

            float scale = Random.Range(minimumScale, maximumScale);
            scenery.transform.localScale = prefab.transform.localScale * scale;

            _spawnedObjects.Add(scenery);
            _placedPositions.Add(position);
            placedCount++;
        }

        if (placedCount < count)
        {
            Debug.LogWarning(
                $"{groupName}: 배치 공간이 부족해 " +
                $"{placedCount}/{count}개를 배치했습니다.",
                this);
        }
    }

    // 연결되지 않은 항목을 제외하고 사용할 Prefab을 모은다.
    private List<GameObject> GetAvailablePrefabs(GameObject[] prefabs)
    {
        List<GameObject> availablePrefabs = new List<GameObject>();


        if (prefabs == null)
        {
            return availablePrefabs;
        }

        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null)
            {
                availablePrefabs.Add(prefab);
            }
        }

        return availablePrefabs;
    }

    private List<Transform> GetAvailablePoints(Transform[] points)
    {
        List<Transform> availablePoints = new List<Transform>();

        if (points == null)
        {
            return availablePoints;
        }

        foreach (Transform point in points)
        {
            if (point != null && !availablePoints.Contains(point))
            {
                availablePoints.Add(point);
            }
        }

        return availablePoints;
    }

    // 후보 지점을 무작위로 선택하고, 기존 배경과 충분히 떨어진 위치를 찾는다.
    // 검사한 후보는 목록에서 제거하므로 모든 후보를 검사하면 종료
    private bool TryGetPosition(
           List<Transform> points,
           out Vector3 position)
    {
        while (points.Count > 0)
        {
            int index = Random.Range(0, points.Count);
            Transform point = points[index];

            // 선택한 후보는 성공 여부와 관계없이 다시 사용하지 않는다.
            points.RemoveAt(index);

            Vector2 offset =
                Random.insideUnitCircle * Mathf.Max(0f, _positionJitter);

            Vector3 candidate =
                point.position + new Vector3(offset.x, offset.y, 0f);

            if (HasSpace(candidate))
            {
                position = candidate;
                return true;
            }
        }

        position = default;
        return false;
    }

    // 오브젝트 중심 사이의 거리만 검사
    // 이미지 전체가 서로 겹치지 않는다는 의미는 아님
    private bool HasSpace(Vector3 position)
    {
        float minimumDistanceSquared =
            _minimumSpacing * _minimumSpacing;

        foreach (Vector3 placedPosition in _placedPositions)
        {
            if ((position - placedPosition).sqrMagnitude
                < minimumDistanceSquared)
            {
                return false;
            }
        }

        return true;
    }
}
