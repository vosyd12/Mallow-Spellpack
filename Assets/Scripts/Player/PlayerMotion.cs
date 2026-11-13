using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    private enum MotionState
    {
        Locomotion,
        Hit,
        Death
    }

    private static readonly int[] AnimationStates =
  {
    Animator.StringToHash("Base.Locomotion"),
    Animator.StringToHash("Base.Hit"),
    Animator.StringToHash("Base.Death")
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

    // Attack 레이어만 관리
    private static readonly int BlendParameter =
    Animator.StringToHash("Blend");

    private static readonly int AttackState =
        Animator.StringToHash("Attack.Attack");

    private static readonly int EmptyState =
        Animator.StringToHash("Attack.Empty");

    private int _attackLayer;
    private bool _isReady;

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
            return;
        }

        _renderers = GetComponentsInChildren<SpriteRenderer>(true);
        _originalSprites = new Sprite[_renderers.Length];
        _originalColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalSprites[i] = _renderers[i].sprite;
            _originalColors[i] = _renderers[i].color;
        }

        _attackLayer = _animator.GetLayerIndex("Attack");

        bool hasStates =
            _attackLayer >= 0 &&
            _animator.HasState(0, AnimationStates[0]) &&
            _animator.HasState(0, AnimationStates[1]) &&
            _animator.HasState(0, AnimationStates[2]);

        if (!hasStates ||
            !_animator.HasState(_attackLayer, AttackState) ||
            !_animator.HasState(_attackLayer, EmptyState))
        {
            Debug.LogError(
                "Player Animator의 레이어와 상태를 확인해주세요.",
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

        _player.Revived += RestoreMotion;
        _player.Damaged += PlayHit;
        _player.Died += PlayDeath;
        _playerAttack.AttackStarted += PlayAttack;

        _hasState = false;
        StopAttackMotion();

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

            if (!_isReady)
            {
                _playerAttack.FinishAttack();
            }
        }

        if (_isReady)
        {
            StopAttackMotion();
            _animator.speed = 1f;
        }
    }

    private void Update()
    {
        if (_player.IsDead)
        {
            return;
        }

        if (!_player.CanAct)
        {
            _animator.speed = 0f;
            return;
        }

        _animator.speed = 1f;

        if (_state == MotionState.Hit ||
            _state == MotionState.Death)
        {
            return;
        }

        PlayLocomotion();
    }

    private void PlayLocomotion()
    {
        ChangeState(MotionState.Locomotion);

        _animator.SetFloat(
            BlendParameter,
            _player.IsMoving ? 0.5f : 0f);
    }

    private void PlayAttack()
    {
        if (!_player.CanAct ||
            _state == MotionState.Death)
        {
            _playerAttack.FinishAttack();
            return;
        }

        PlayLocomotion();

        _animator.SetLayerWeight(_attackLayer, 1f);
        _animator.Play(AttackState, _attackLayer, 0f);
    }

    private void StopAttackMotion()
    {
        _playerAttack.FinishAttack();

        _animator.Play(EmptyState, _attackLayer, 0f);
        _animator.SetLayerWeight(_attackLayer, 0f);
    }

    private void PlayHit()
    {
        if (!_player.CanAct || _playerAttack.IsAttacking)
        {
            return;
        }

        ChangeState(MotionState.Hit, true);
    }

    private void PlayDeath()
    {
        if (_hasState && _state == MotionState.Death)
        {
            return;
        }

        StopAttackMotion();

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

    public void FireProjectile()
    {
        if (!_player.CanAct || !_playerAttack.IsAttacking)
        {
            return;
        }

        _playerAttack.MagicAttack();
    }

    private void RestoreMotion()
    {
        _playerAttack.ResetAttack();

        // 사망 클립의 이동·회전·배율 등 바인딩 상태 초기화
        _animator.Rebind();

        _hasState = false;

        StopAttackMotion();
        PlayLocomotion();

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

    // Attack과 Hit 클립의 종료 시점에 호출한다.
    public void FinishAttackMotion()
    {
        if (_player.IsDead || !_playerAttack.IsAttacking)
        {
            return;
        }

        StopAttackMotion();
    }

    public void FinishHitMotion()
    {
        if (_player.IsDead || _state != MotionState.Hit)
        {
            return;
        }

        PlayLocomotion();
    }
}