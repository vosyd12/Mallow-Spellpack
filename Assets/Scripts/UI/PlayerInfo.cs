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
        string name,
        int level,
        int exp,
        int levelExp)
    {
        _nameText.text = name;
        _levelText.text = $"Lv. {level}";

        float ratio = levelExp > 0
            ? Mathf.Clamp01((float)exp / levelExp)
            : 0f;

        _expText.text = $"{exp} / {levelExp}";

        _expFill.fillAmount = ratio;

        // 충분한 경험치가 있을 때만 클릭할 수 있다.
        _levelUpButton.interactable =
            levelExp > 0 && exp >= levelExp;
    }
}