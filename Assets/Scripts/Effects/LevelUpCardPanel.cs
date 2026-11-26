using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] 
    private Player _player;

    [SerializeField] 
    private GameObject _cardRoot;

    [SerializeField] 
    private Button[] _cardButtons;

    [SerializeField]
    private PlayerSkill _playerSkill;

    [SerializeField]
    private TMP_Text[] _cardLabels = new TMP_Text[3];

    [SerializeField]
    private Image[] _cardIcons = new Image[3];
    private readonly List<SkillBehaviour> _cardChoices = new();

    private Animator _animator;
    private PanelState _state;
    private float _timeScaleBeforeCard;

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
            _playerSkill == null ||
            !PrepareCards())
        {
            return;
        }


        // 현재 시간 배율을 저장하고 카드 선택 중 전투를 멈춘다.
        _timeScaleBeforeCard = Time.timeScale;
        _state = PanelState.Opening;
        _player.SetLevelUpCardOpen(true);
        Time.timeScale = 0f;

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
            cardIndex >= _cardChoices.Count ||
            !_playerSkill.TryChooseSkill(_cardChoices[cardIndex]))
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

        // 카드를 열기 전 배율로 복원하고 다음 레벨업을 허용한다.
        Time.timeScale = _timeScaleBeforeCard;
        _player.SetLevelUpCardOpen(false);

        _animator.ResetTrigger(OpenTrigger);
        _animator.ResetTrigger(ConfirmTrigger);
        _cardRoot.SetActive(false);
    }

    private void SetCardButtonsInteractable(bool interactable)
    {
        for (int i = 0; i < _cardButtons.Length; i++)
        {
            _cardButtons[i].interactable = interactable && i < _cardChoices.Count;
        }
    }

    private bool PrepareCards()
    {
        _playerSkill.GetCardCandidates(_cardChoices);

        int count = Mathf.Min(_cardChoices.Count, _cardButtons.Length);

        // 이번 레벨업에 표시할 카드를 중복 없이 뽑는다.
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(i, _cardChoices.Count);
            (_cardChoices[i], _cardChoices[randomIndex]) =
                (_cardChoices[randomIndex], _cardChoices[i]);
        }

        _cardChoices.RemoveRange(count, _cardChoices.Count - count);

        for (int i = 0; i < _cardButtons.Length; i++)
        {
            bool hasChoice = i < count;
            _cardButtons[i].gameObject.SetActive(hasChoice);

            SkillBehaviour skill = hasChoice ? _cardChoices[i] : null;

            // 카드가 열리기 전에 슬롯마다 뽑힌 스킬의 이미지를 넣는다.
            if (i < _cardIcons.Length && _cardIcons[i] != null)
            {
                Image icon = _cardIcons[i];
                icon.sprite = skill != null ? skill.Data._iconSprite : null;
                icon.enabled = icon.sprite != null;
            }

            if (skill == null || i >= _cardLabels.Length || _cardLabels[i] == null)
            {
                continue;
            }

            string action = skill.IsLearned
                ? $"강화 · Lv. {skill.Data._level + 1}"
                : "획득 · Lv. 1";

            _cardLabels[i].text = $"{skill.Data._skillId}\n{action}";
        }

        return count > 0;
    }
}