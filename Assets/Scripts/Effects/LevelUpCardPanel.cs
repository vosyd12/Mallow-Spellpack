using UnityEngine;

public class PlayerLevelUpEffect : MonoBehaviour
{
    private static readonly int LevelUpState =
        Animator.StringToHash("Base Layer.LevelUp");

    [SerializeField]
    private ParticleSystem _lightPillars;
    [SerializeField]
    private Animator _effectAnimator;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();

        if (_lightPillars == null ||
            _effectAnimator == null)
        {
            Debug.LogError("레벨업 파티클과 Animator를 연결해 주세요.", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        _player.LeveledUp += Play;
    }

    private void OnDisable()
    {
        _player.LeveledUp -= Play;
    }

    private void Play()
    {
        _lightPillars.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _lightPillars.Play(true);

        _effectAnimator.Play(LevelUpState, 0, 0f);
    }
}