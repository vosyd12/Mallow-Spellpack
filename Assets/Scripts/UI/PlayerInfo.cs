using TMPro;
using UnityEngine;
using UnityEngine.UI;

// UI 표시용 PlayerInfo
public class PlayerInfo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _nameText;

    [SerializeField]
    private TMP_Text _levelText;

    [SerializeField]
    private Image _expFill;

    [SerializeField]
    private TMP_Text _expText;

    [SerializeField]
    private Button _levelUpButton;

    public void SetInfo(
        int level,
        int exp,
        int levelExp)
    {
        _levelText.text = $"Lv. {level}";

        float ratio = levelExp > 0
            ? Mathf.Clamp01((float)exp / levelExp)
            : 0f;

        _expText.text = $"{exp} / {levelExp}";

        _expFill.fillAmount = ratio;

        // 레벨업 버튼이 있는 씬에서만 클릭 가능 상태를 갱신한다.
        if (_levelUpButton != null)
        {
            _levelUpButton.interactable = levelExp > 0 && exp >= levelExp;
        }
    }
}