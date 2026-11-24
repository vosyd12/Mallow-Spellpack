using UnityEngine.UI;
using UnityEngine;

public class LevelUpCardPanel : MonoBehaviour
{
    private enum PanelState
    {
        Closed,
        Opening,
        Choosing,
        Confirming
    }

    private static readonly int OpenTrigger = Animator.StringToHash("Open");
    private static readonly int ConfirmTrigger = Animator.StringToHash("Confirm");
    private static readonly int SelectedCard = Animator.StringToHash("SelectedCard");

    [SerializeField] private Player _player;
    [SerializeField] private GameObject _cardRoot;
    [SerializeField] private Button[] _cardButtons;
    [SerializeField] private CanvasGroup _levelUpButtonGroup;

    [SerializeField]
    private PlayerSkill _playerSkill;

    private Animator _animator;
    private PanelState _state;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _cardRoot.SetActive(false);
    }

    private void OnEnable()
    {
        _player.LeveledUp += Open;
    }

    private void OnDisable()
    {
        _player.LeveledUp -= Open;

        if (_state != PanelState.Closed)
        {
            Close();
        }
    }

    private void Open()
    {
        if (_state != PanelState.Closed ||
            (!CanSelectCard(0) && !CanSelectCard(1)))
        {
            return;
        }


        _state = PanelState.Opening;
        _levelUpButtonGroup.interactable = false;
        SetCardButtonsInteractable(false);
        _cardRoot.SetActive(true);
        _animator.SetTrigger(OpenTrigger);
    }

    // 등장 클립의 마지막 Animation Event
    public void FinishOpening()
    {
        if (_state != PanelState.Opening)
        {
            return;
        }

        _state = PanelState.Choosing;
        SetCardButtonsInteractable(true);
    }

    // 각 카드 버튼의 On Click에서 호출
    public void ConfirmSelection(int cardIndex)
    {
        if (_state != PanelState.Choosing ||
            cardIndex < 0 || 
            cardIndex >= _cardButtons.Length)
        {
            return;
        }

        var target = GetUpgradeTarget(cardIndex);

        if (!target.HasValue || 
            !_playerSkill.TryUpgrade(target.Value))
        {
            return;
        }

        _state = PanelState.Confirming;
        SetCardButtonsInteractable(false);
        _animator.SetInteger(SelectedCard, cardIndex);
        _animator.SetTrigger(ConfirmTrigger);
    }

    // 확정 클립의 마지막 Animation Event
    public void FinishConfirmation()
    {
        if (_state == PanelState.Confirming)
        {
            Close();
        }
    }

    private void Close()
    {
        _state = PanelState.Closed;
        _animator.ResetTrigger(OpenTrigger);
        _animator.ResetTrigger(ConfirmTrigger);
        _cardRoot.SetActive(false);
        _levelUpButtonGroup.interactable = true;
    }

    private void SetCardButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < _cardButtons.Length; i++)
        {
            _cardButtons[i].interactable = interactable && CanSelectCard(i);
        }
    }

    private PlayerSkill.UpgradeTarget? GetUpgradeTarget(int cardIndex)
    {
        return cardIndex switch
        {
            0 => PlayerSkill.UpgradeTarget.WaterBall,
            1 => PlayerSkill.UpgradeTarget.FireArea,
            _ => null
        };
    }

    private bool CanSelectCard(int cardIndex)
    {
        var target = GetUpgradeTarget(cardIndex);
        return target.HasValue && _playerSkill.CanUpgrade(target.Value);
    }
}