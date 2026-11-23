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
        float gaugeRatio,
        bool showingBossHealth = false)
    {
        _stageNumberText.text = $"STAGE {stageNumber:00}";

        float ratio = Mathf.Clamp01(gaugeRatio);
        _bossFill.fillAmount = ratio;

        // 처치 게이지가 가득 찼을 때만 등장 안내를 표시한다.
        _bossAppearText.gameObject.SetActive(!showingBossHealth && ratio >= 1f);
    }
}