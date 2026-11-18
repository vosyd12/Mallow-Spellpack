using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillSlotUI : MonoBehaviour
{
    private enum SlotSkill
    {
        Empty,
        WaterBall,
        FireArea
    }

    [SerializeField] private PlayerSkill _playerSkill;
    [SerializeField] private SlotSkill _skill;

    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _emptyState;
    [SerializeField] private Image _cooldownOverlay;
    [SerializeField] private TMP_Text _cooldownText;

    private Button _button;
    private int _lastSeconds = -1;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(HandleClick);

        bool isAssigned = _skill != SlotSkill.Empty;

        if (_emptyState != null)
        {
            _emptyState.SetActive(!isAssigned);
        }

        if (_icon != null)
        {
            _icon.gameObject.SetActive(
                isAssigned && _icon.sprite != null);
        }

        RefreshState();
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(HandleClick);
    }

    private void Update()
    {
        RefreshState();
    }

    private void HandleClick()
    {
        if (_playerSkill == null ||
            !_playerSkill.isActiveAndEnabled)
        {
            return;
        }

        switch (_skill)
        {
            case SlotSkill.WaterBall:
                _playerSkill.UseWaterBall();
                break;

            case SlotSkill.FireArea:
                _playerSkill.UseFireArea();
                break;
        }

        RefreshState();
    }

    private void RefreshState()
    {
        float remaining = 0f;
        float duration = 0f;
        bool canUse = false;

        if (_playerSkill != null &&
            _playerSkill.isActiveAndEnabled)
        {
            switch (_skill)
            {
                case SlotSkill.WaterBall:
                    remaining = _playerSkill.WaterBallCooldownRemaining;
                    duration = _playerSkill.WaterBallCooldownDuration;
                    canUse = _playerSkill.CanManuallyUseWaterBall;
                    break;

                case SlotSkill.FireArea:
                    remaining = _playerSkill.FireAreaCooldownRemaining;
                    duration = _playerSkill.FireAreaCooldownDuration;
                    canUse = _playerSkill.CanUseFireArea;
                    break;
            }
        }

        _button.interactable = canUse;

        bool isCoolingDown = remaining > 0f;

        if (_cooldownOverlay != null)
        {
            _cooldownOverlay.gameObject.SetActive(isCoolingDown);

            _cooldownOverlay.fillAmount = duration > 0f
                ? Mathf.Clamp01(remaining / duration)
                : 0f;
        }

        if (_cooldownText != null)
        {
            _cooldownText.gameObject.SetActive(isCoolingDown);

            int seconds = Mathf.CeilToInt(remaining);

            if (seconds != _lastSeconds)
            {
                _lastSeconds = seconds;
                _cooldownText.text = seconds.ToString();
            }
        }
    }
}