
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class LandingDustPool : MonoBehaviour
{
    [Serializable]
    private class ElementDust
    {
        public Element Element;
        public LandingDust Prefab;
    }

    [SerializeField]
    private ElementDust[] _elementDusts;

    [SerializeField, Min(0)]
    private int _prewarmCountPerElement = 6;

    [SerializeField, Min(1)]
    private int _maxPoolSizePerElement = 16;

    public static LandingDustPool Instance { get; private set; }
    private readonly Dictionary<Element, ObjectPool<LandingDust>> _pools =
        new Dictionary<Element, ObjectPool<LandingDust>>();

    private void Awake()
    {
        Instance = this;

        foreach (ElementDust entry in _elementDusts)
        {
            if (entry == null ||
                entry.Prefab == null ||
                entry.Element == Element.Max)
            {
                Debug.LogError("착지 효과의 속성과 Prefab을 확인하세요.", this);
                continue;
            }

            if (_pools.ContainsKey(entry.Element))
            {
                Debug.LogError(
                    $"착지 효과의 속성이 중복되었습니다: {entry.Element}",
                    this);

                continue;
            }

            ObjectPool<LandingDust> pool = CreatePool(entry.Prefab);

            _pools.Add(entry.Element, pool);
            Prewarm(pool);
        }
    }

    private ObjectPool<LandingDust> CreatePool(LandingDust prefab)
    {
        ObjectPool<LandingDust> pool = null;

        pool = new ObjectPool<LandingDust>(
            createFunc: () =>
            {
                LandingDust dust = Instantiate(prefab, transform);

                dust.SetPool(pool);
                dust.gameObject.SetActive(false);

                return dust;
            },
            actionOnRelease: dust => dust.gameObject.SetActive(false),
            actionOnDestroy: dust => Destroy(dust.gameObject),
            defaultCapacity: _prewarmCountPerElement,
            maxSize: _maxPoolSizePerElement);

        return pool;
    }

    private void Prewarm(ObjectPool<LandingDust> pool)
    {
        int count = Mathf.Min(
            _prewarmCountPerElement,
            _maxPoolSizePerElement);

        LandingDust[] prepared = new LandingDust[count];

        for (int i = 0; i < prepared.Length; i++)
        {
            prepared[i] = pool.Get();
        }

        for (int i = 0; i < prepared.Length; i++)
        {
            pool.Release(prepared[i]);
        }
    }

    public void Play(
        Element element,
        Vector3 position,
        float landingSpeed,
        int particleCount,
        float spreadMultiplier = 1f)
    {
        if (!_pools.TryGetValue(element, out ObjectPool<LandingDust> pool))
        {
            Debug.LogWarning(
                $"등록되지 않은 착지 효과입니다: {element}",
                this);

            return;
        }

        LandingDust dust = pool.Get();

        dust.transform.position = position;
        dust.gameObject.SetActive(true);
        dust.Play(landingSpeed, particleCount, spreadMultiplier);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        foreach (ObjectPool<LandingDust> pool in _pools.Values)
        {
            pool.Clear();
        }

        _pools.Clear();
    }
}