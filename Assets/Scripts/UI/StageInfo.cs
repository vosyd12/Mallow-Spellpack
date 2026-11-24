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

    [SerializeField] 
    private GameObject _bossAnnouncementRoot;
    [SerializeField] 
    private Animator _bossTimerAnimator;
    [SerializeField] 
    private TMP_Text _bossTimerText;

    private int _showBossSeconds = -1;

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

    public void ShowBossIntro(int initialSeconds)
    {
        // 타이틀은 루트 활성화 시 기본 애니메이션을 재생한다.
        _bossAnnouncementRoot.SetActive(true);
        _bossTimerAnimator.gameObject.SetActive(true);
        SetBossTimerSeconds(initialSeconds);
    }

    public void StartBossTimerAnimation()
    {
        _bossTimerAnimator.SetTrigger("StartCountdown");
    }

    public void SetBossTimerSeconds(int seconds)
    {
        if(_showBossSeconds == seconds)
        {
            return;
        }

        _showBossSeconds = seconds;
        _bossTimerText.text = $"{seconds / 60:00}: {seconds % 60:00}";
    }

    public void HideBossPresentation()
    {
        _bossAnnouncementRoot.SetActive(false);
        _bossTimerAnimator.gameObject.SetActive(false);

        _showBossSeconds = -1;
    }
}