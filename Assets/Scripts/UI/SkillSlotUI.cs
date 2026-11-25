using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    private enum SlotSkill
    {
        Empty,
        WaterBall,
        FireArea
    }

    [SerializeField] 
    private PlayerSkill _playerSkill;

    [SerializeField] 
    private SlotSkill _skill;

    [SerializeField] 
    private Image _icon;
    [SerializeField]
    private Image _cooldownOverlay;
    [SerializeField] 
    private TMP_Text _cooldownText;

    private int _lastSeconds = -1;


    private void OnEnable()
    {
        if (_icon != null)
        {
            // 연결된 스킬에 스프라이트가 있을 때 아이콘을 표시한다.
            _icon.gameObject.SetActive(
                _skill != SlotSkill.Empty && _icon.sprite != null);
        }

        RefreshState();
    }

    private void Update()
    {
        RefreshState();
    }

    private void RefreshState()
    {
        float remaining = 0f;
        float duration = 0f;

        if (_playerSkill != null &&
            _playerSkill.isActiveAndEnabled)
        {
            switch (_skill)
            {
                case SlotSkill.WaterBall:
                    remaining = _playerSkill.WaterBallCooldownRemaining;
                    duration = _playerSkill.WaterBallCooldownDuration;
                    break;

                case SlotSkill.FireArea:
                    remaining = _playerSkill.FireAreaCooldownRemaining;
                    duration = _playerSkill.FireAreaCooldownDuration;
                    break;
            }
        }

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