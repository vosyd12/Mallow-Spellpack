using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int _stageNumber = 1;

    [SerializeField]
    private StageScenery _stageScenery;

    [SerializeField]
    private StageInfo _stageInfo;

    private float _bossProgress;

    public int StageNumber => _stageNumber;

    private void Start()
    {
        EnterStage(_stageNumber);
    }

    public void EnterStage(int stageNumber)
    {
        if (stageNumber < 1)
        {
            Debug.LogWarning("스테이지 번호는 1 이상이여야 합니다.", this);
            return;
        }

        if (_stageScenery == null)
        {
            Debug.LogError("StageScenery를 연결해주세요.", this);
            return;
        }

        _stageNumber = stageNumber;
        _bossProgress = 0f;

        _stageScenery.Rebuild();
        RefreshStageInfo();
    }

    public void SetBossProgress(float progress)
    {
        _bossProgress = Mathf.Clamp01(progress);

        RefreshStageInfo();
    }

    private void RefreshStageInfo()
    {
        if(_stageInfo == null)
        {
            return;
        }

        _stageInfo.SetInfo(
            _stageNumber,
            _bossProgress);
    }
}