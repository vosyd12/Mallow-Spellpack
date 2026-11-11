using UnityEngine;

public class HitReaction : MonoBehaviour
{
    private static readonly int IdleState =
        Animator.StringToHash("Base Layer.Idle");

    private static readonly int HitState =
        Animator.StringToHash("Base Layer.Hit");

    [SerializeField]
    private Animator _effectAnimator;

    [SerializeField]
    private HitEffectVisual _effectVisual;

    private bool _isReady;

    private void Awake()
    {
        if (_effectAnimator == null ||
            _effectVisual == null ||
            _effectAnimator.runtimeAnimatorController == null)
        {
            Debug.LogError(
                "HitReaction의 효과용 Animator와 Visual을 연결해주세요.",
                this);

            enabled = false;
            return;
        }

        _isReady = true;
    }

    private void OnEnable()
    {
        if (!_isReady)
        {
            return;
        }

        _effectAnimator.enabled = true;
        _effectAnimator.Play(IdleState, 0, 0f);

        _effectVisual.ResetEffect();
    }

    public void Play()
    {
        if (!_isReady || !isActiveAndEnabled)
        {
            return;
        }

        // 연속 피격도 처음부터 다시 재생한다.
        _effectAnimator.Play(HitState, 0, 0f);
    }

    private void OnDisable()
    {
        // Animation이 초기화한 값을 다시 덮어쓰지 않도록
        // 효과용 Animator를 먼저 중지한다.
        if (_isReady)
        {
            _effectAnimator.enabled = false;
            _effectVisual.ResetEffect();
        }
    }
}