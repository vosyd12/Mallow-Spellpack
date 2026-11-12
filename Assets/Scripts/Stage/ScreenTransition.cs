using System.Collections;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    // 위에서 아래로 검은 영역 확장
    private static readonly int CoverState =
        Animator.StringToHash("Base Layer.Cover");

    // 전체가 검은 화면
    private static readonly int RevealState =
        Animator.StringToHash("Base Layer.Reveal");

    [SerializeField]
    private Animator _animator;

    [SerializeField, Min(1f)]
    private float _maxWaitDuration = 10f;

    public bool Succeeded { get; private set; }


    private bool _isFinished;

    public IEnumerator Cover()
    {
        yield return PlayTransition(CoverState);
    }

    public IEnumerator Reveal()
    {
        yield return PlayTransition(RevealState);
    }

    private IEnumerator PlayTransition(int state)
    {
        Succeeded = false;
        _isFinished = false;

        if (_animator == null ||
            !_animator.isActiveAndEnabled ||
            _animator.runtimeAnimatorController == null ||
            !_animator.HasState(0, state))
        {
            Debug.LogError("화면 전환 Animator와 상태를 확인해주세요.", this);
            yield break;
        }

        _animator.Play(state, 0, 0f);
        _animator.Update(0f);

        float elapsed = 0f;

        while (elapsed < _maxWaitDuration)
        {
            if (!_animator.isActiveAndEnabled)
            {
                break;
            }

            AnimatorStateInfo info =
                _animator.GetCurrentAnimatorStateInfo(0);

            if (info.fullPathHash != state)
            {
                break;
            }

            if (_isFinished || (!info.loop && info.normalizedTime >= 1f))
            {
                Succeeded = true;
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.LogError("화면 전환이 정상적으로 완료되지 않았습니다.", this);
    }

    // Cover·Reveal 클립 마지막의 Animation Event에서 호출
    public void FinishTransition()
    {
        _isFinished = true;
    }
}