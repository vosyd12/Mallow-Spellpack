using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    private enum MotionState
    {
        Idle,
        Walk,
        Attack,
        Hit,
        Death
    }

    private static readonly int[] AnimationStates =
    {
        Animator.StringToHash("Base Layer.Idle"),
        Animator.StringToHash("Base Layer.Walk"),
        Animator.StringToHash("Base Layer.Attack"),
        Animator.StringToHash("Base Layer.Hit"),
        Animator.StringToHash("Base Layer.Death")
    };

    [SerializeField] private Player _player;
    [SerializeField] private PlayerAttack _playerAttack;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _groundShadow;

    private SpriteRenderer[] _renderers;
    private Sprite[] _originalSprites;
    private Color[] _originalColors;

    private MotionState _state;
    private bool _hasState;

    private void Awake()
    {
        if (_player == null)
        {
            _player = GetComponentInParent<Player>();
        }

        if (_playerAttack == null && 
            _player != null)
        {
            _playerAttack = _player.GetComponent<PlayerAttack>();
        }

        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }

        if (_player == null ||
            _playerAttack == null ||
            _animator == null ||
            _animator.runtimeAnimatorController == null)
        {
            Debug.LogError(
                "PlayerMotion의 Player, PlayerAttack, Animator를 확인해주세요.",
                this);

            enabled = false;
        }

        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _originalSprites = new Sprite[_renderers.Length];
        _originalColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalSprites[i] = _renderers[i].sprite;
            _originalColors[i] = _renderers[i].color;
        }
    }

    private void OnEnable()
    {
        _player.Revived += RestoreMotion;
        _player.Damaged += PlayHit;
        _player.Died += PlayDeath;
        _playerAttack.AttackStarted += PlayAttack;

        _hasState = false;

        if (_player.IsDead)
        {
            PlayDeath();
            return;
        }

        PlayLocomotion();
    }

    private void OnDisable()
    {
        if (_player != null)
        {
            _player.Revived -= RestoreMotion;
            _player.Damaged -= PlayHit;
            _player.Died -= PlayDeath;
        }

        if (_playerAttack != null)
        {
            _playerAttack.AttackStarted -= PlayAttack;
            _playerAttack.FinishAttack();
        }
    }

    private void Update()
    {
        if (_player.IsDead ||
            _state == MotionState.Attack ||
            _state == MotionState.Hit ||
            _state == MotionState.Death)
        {
            return;
        }

        PlayLocomotion();
    }

    private void PlayLocomotion()
    {
        ChangeState(
            _player.IsMoving
                ? MotionState.Walk
                : MotionState.Idle);
    }

    private void PlayAttack()
    {
        if (_player.IsDead)
        {
            _playerAttack.FinishAttack();
            return;
        }

        ChangeState(MotionState.Attack, true);
    }

    private void PlayHit()
    {
        if (_player.IsDead || _state == MotionState.Attack)
        {
            return;
        }

        // 연속 피격이면 Hit 동작을 처음부터 다시 재생한다.
        ChangeState(MotionState.Hit, true);
    }

    private void PlayDeath()
    {
        if (_hasState && _state == MotionState.Death)
        {
            return;
        }

        if (_groundShadow != null)
        {
            _groundShadow.SetActive(false);
        }

        ChangeState(MotionState.Death);
    }

    private void ChangeState(
        MotionState state,
        bool restart = false)
    {
        if (_hasState && _state == state && !restart)
        {
            return;
        }

        _state = state;
        _hasState = true;

        _animator.speed = 1f;
        _animator.Play(AnimationStates[(int)state], 0, 0f);
    }

    // Attack 클립의 발사 시점에 Animation Event로 호출한다.
    public void FireProjectile()
    {
        if (_player.IsDead || _state != MotionState.Attack)
        {
            return;
        }

        _playerAttack.MagicAttack();
    }

    // Attack과 Hit 클립의 종료 시점에 호출한다.
    public void FinishAction()
    {
        if (_player.IsDead ||
            (_state != MotionState.Attack &&
             _state != MotionState.Hit))
        {
            return;
        }

        if (_state == MotionState.Attack)
        {
            _playerAttack.FinishAttack();
        }

        PlayLocomotion();
    }

    private void RestoreMotion()
    {
        _playerAttack.ResetAttack();

        // 사망 클립의 이동·회전·배율 등 바인딩 상태 초기화
        _animator.Rebind();

        _hasState = false;
        ChangeState(MotionState.Idle);
        _animator.Update(0f);

        // 사망 표정과 페이드 잔여 값 복구
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].sprite = _originalSprites[i];
            _renderers[i].color = _originalColors[i];
        }

        if (_groundShadow != null)
        {
            _groundShadow.SetActive(true);
        }
    }
}