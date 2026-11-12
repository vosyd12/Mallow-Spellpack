
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(ParticleSystem))]
public class LandingDust : MonoBehaviour
{
    [SerializeField, Min(0.01f)]
    private float _referenceLandingSpeed = 2f;

    [SerializeField, Min(0f)]
    private float _horizontalSpeed = 1.2f;

    [SerializeField]
    private float _minimumVerticalSpeed = 0.05f;

    [SerializeField]
    private float _verticalSpeed = 0.35f;

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

    public void SetPool(IObjectPool<LandingDust> pool)
    {
        _pool = pool;
    }

    public void Play(
        float landingSpeed,
        int particleCount,
        float spreadMultiplier = 1f)
    {
        _isInUse = true;
        _particles.Clear(true);

        float strength = Mathf.Clamp(
            landingSpeed / _referenceLandingSpeed,
            0.5f,
            2f);

        float spread = Mathf.Clamp(
            spreadMultiplier,
            1f,
            3f);

        float speedMultiplier = strength * spread;

        var main = _particles.main;

        particleCount = Mathf.Clamp(
            particleCount,
            1,
            main.maxParticles);

        ApplyVelocity(speedMultiplier);

        var emission = _particles.emission;

        emission.SetBurst(
            0,
            new ParticleSystem.Burst(0f, (short)particleCount));

        _particles.Play();
    }

    private void ApplyVelocity(float multiplier)
    {
        var velocity = _particles.velocityOverLifetime;
        velocity.enabled = true;

        velocity.x = new ParticleSystem.MinMaxCurve(
            -_horizontalSpeed * multiplier,
            _horizontalSpeed * multiplier);

        float minimum = Mathf.Min(
            _minimumVerticalSpeed,
            _verticalSpeed);

        float maximum = Mathf.Max(
            _minimumVerticalSpeed,
            _verticalSpeed);

        velocity.y = new ParticleSystem.MinMaxCurve(
            minimum * multiplier,
            maximum * multiplier);
    }

    private void OnParticleSystemStopped()
    {
        if (!_isInUse)
        {
            return;
        }

        _isInUse = false;
        _pool.Release(this);
    }

    public void StopAndReturn()
    {
        if (!_isInUse)
        {
            return;
        }

        // Stop 콜백에서 중복 반환하지 않도록 먼저 변경한다.
        _isInUse = false;

        _particles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear);

        _pool.Release(this);
    }
}