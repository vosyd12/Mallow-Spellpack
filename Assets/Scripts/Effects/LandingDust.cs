using UnityEngine;
using UnityEngine.Pool;

public class LandingDust : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float _referenceLandingSpeed = 2f;

    [SerializeField] private float _horizontalSpeed = 1.2f;
    [SerializeField] private float _verticalSpeed = 0.35f;

    private ParticleSystem _particles;
    private IObjectPool<LandingDust> _pool;
    private bool _isInUse;

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();

        var main = _particles.main;
        main.playOnAwake = false;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    public void Play(float landingSpeed, int particleCount, float spreadMultiplier = 1f)
    {
        _isInUse = true;
        _particles.Clear(true);

        // 착지 속도가 기준값보다 높으면 먼지가 더 빠르게 퍼지고 높게 올라간다.
        float strength = Mathf.Clamp(
            landingSpeed / _referenceLandingSpeed,
            0.5f,
            2f);

        var main = _particles.main;

        particleCount = Mathf.Clamp(
            particleCount, 1, main.maxParticles);

        // 입자마다 좌우·위쪽 속도를 무작위로 선택한다.
        float spread = Mathf.Clamp(spreadMultiplier, 1f, 3f);
        var velocity = _particles.velocityOverLifetime;

        velocity.x = new ParticleSystem.MinMaxCurve(
            -_horizontalSpeed * strength * spread,
            _horizontalSpeed * strength * spread);

        velocity.y = new ParticleSystem.MinMaxCurve(
            0.05f * strength * spread,
            _verticalSpeed * strength * spread);

        // 준비된 Prefab의 Burst 개수만 변경한다.
        var emission = _particles.emission;

        emission.SetBurst(
            0,
            new ParticleSystem.Burst(0f, (short)particleCount));

        _particles.Play();
    }

    public void SetPool(IObjectPool<LandingDust> pool)
    {
        _pool = pool;
    }

    private void OnParticleSystemStopped()
    {
        if (!_isInUse)
            return;

        _isInUse = false;
        _pool.Release(this);
    }
}