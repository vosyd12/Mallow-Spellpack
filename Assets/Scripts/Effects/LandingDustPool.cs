
using UnityEngine;
using UnityEngine.Pool;

public class LandingDustPool : MonoBehaviour
{
    [SerializeField] private LandingDust _prefab;

    public static LandingDustPool Instance { get; private set; }
    private ObjectPool<LandingDust> _pool;

    private void Awake()
    {
        Instance = this;

        _pool = new ObjectPool<LandingDust>(
            createFunc: CreateDust,
            actionOnRelease: dust => dust.gameObject.SetActive(false),
            actionOnDestroy: dust => Destroy(dust.gameObject),
            defaultCapacity: 40,
            maxSize: 64);

        // 시작할 때 40개를 미리 생성해 둔다.
        LandingDust[] prepared = new LandingDust[40];

        for (int i = 0; i < prepared.Length; i++)
        {
            prepared[i] = _pool.Get();
        }

        for (int i = 0; i < prepared.Length; i++)
        {
            _pool.Release(prepared[i]);
        }
    }

    private LandingDust CreateDust()
    {
        LandingDust dust = Instantiate(_prefab, transform);

        dust.SetPool(_pool);
        dust.gameObject.SetActive(false);

        return dust;
    }

    public void Play(
        Vector3 position,
        float landingSpeed,
        int particleCount)
    {
        LandingDust dust = _pool.Get();

        dust.transform.position = position;
        dust.gameObject.SetActive(true);
        dust.Play(landingSpeed, particleCount);
    }

    private void OnDestroy()
    {
        if (Instance == this) { Instance = null; }

        _pool?.Clear();
    }
}