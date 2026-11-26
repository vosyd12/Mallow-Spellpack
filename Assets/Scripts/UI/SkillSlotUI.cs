using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] 
    private PlayerSkill _playerSkill;

    [SerializeField, Min(0)]
    private int _slotIndex;

    [SerializeField] 
    private Image _icon;
    [SerializeField]
    private Image _cooldownOverlay;
    [SerializeField] 
    private TMP_Text _cooldownText;

    private int _lastSeconds = -1;


    private void OnEnable()
    {
        RefreshState();
    }

    private void Update()
    {
        RefreshState();
    }

    private void RefreshState()
    {
        SkillBehaviour skill = _playerSkill != null ?
            _playerSkill.GetLearnedSkill(_slotIndex) : null;

        // 아직 획득하지 않은 슬롯은 아이콘과 쿨타임을 숨긴다.
        Sprite icon = skill != null ? skill.Data._iconSprite : null;
        float remaining = skill != null ? skill.CooldownRemaining : 0f;
        float duration = skill != null ? skill.CooldownDuration : 0f;
        bool isCoolingDown = remaining > 0f && duration > 0f;

        if (_icon != null)
        {
            _icon.sprite = icon;
            _icon.enabled = icon != null;
        }

        if (_cooldownOverlay != null)
        {
            _cooldownOverlay.enabled = isCoolingDown;
            _cooldownOverlay.fillAmount = isCoolingDown
                ? Mathf.Clamp01(remaining / duration)
                : 0f;
            _cooldownOverlay.gameObject.SetActive(isCoolingDown);
        }

        if (_cooldownText != null)
        {
            _cooldownText.enabled = isCoolingDown;
            _cooldownText.gameObject.SetActive(isCoolingDown);

            int seconds = Mathf.CeilToInt(remaining);
            if (isCoolingDown && seconds != _lastSeconds)
            {
                _lastSeconds = seconds;
                _cooldownText.text = seconds.ToString();
            }
        }
    }
}