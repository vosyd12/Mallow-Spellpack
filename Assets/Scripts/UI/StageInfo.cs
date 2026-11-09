using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _stageNumberText;
    [SerializeField]
    private Image _bossFill;
    [SerializeField]
    private TMP_Text _bossAppearText;

    [SerializeField]
    private Image _goldImage;

    [SerializeField]
    private TMP_Text _goldText;

    public void SetInfo(
        int stageNumber,
        float bossProgress)
    {
        _stageNumberText.text = $"STAGE {stageNumber:00}";

        float ratio = Mathf.Clamp01(bossProgress);

        _bossFill.fillAmount = ratio;

        // 게이지가 가득 차면 등장 안내 문구를 표시한다.
        _bossAppearText.gameObject.SetActive(ratio >= 1f);
    }
}