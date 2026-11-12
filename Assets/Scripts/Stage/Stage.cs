using System.Collections;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int _stageNumber = 1;

    [SerializeField]
    private StageScenery _stageScenery;

    [SerializeField]
    private StageInfo _stageInfo;

    [SerializeField, Min(1)]
    private int _killsForBoss = 30;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private ScreenTransition _screenTransition;

    [SerializeField, Min(0f)]
    private float _blackHoldDuration = 0.5f;

    private bool _isRestarting;

    public bool IsRestarting => _isRestarting;

    private int _killCount;

    private int _stageMaxKill = 100;

    private float _bossProgress;

    public int StageNumber => _stageNumber;

    private void Start()
    {
        EnterStage(_stageNumber);
    }

    private void Update()
    {
        GameOver();
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
        _killCount = 0;
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

    public void GameOver()
    {
        // 플레이어가 죽었는지 감지한다.
        if(_isRestarting ||
           _player == null || 
           !_player.IsDead)
        {
            return;
        }

        if (_screenTransition == null)
        {
            return;   
        }

        _isRestarting = true;
        StartCoroutine(RestartRoutine());
    }

    public void BossGauge()
    {
        if( _isRestarting || 
            _killCount >= _killsForBoss)
        {
             return;
        }

        _killCount++;
        SetBossProgress((float)_killCount / _killsForBoss);
    }

    public void CompleteStage()
    {
        _killsForBoss = Mathf.Min(_killsForBoss + 1, _stageMaxKill);

        EnterStage(_stageNumber + 1);
    }

    private IEnumerator RestartRoutine()
    {
        // 위에서 아래로 완전히 덮을 때까지 대기
        yield return _screenTransition.Cover();

        if(!_screenTransition.Succeeded)
        {
            yield break;
        }

        ResetBossGauge();

        // HP·사망 상태·물리·체력바·외형 복구
        // 전투 행동은 IsRestarting이 false가 된 뒤 재개
        _player.Revive();

        yield return new WaitForSecondsRealtime(_blackHoldDuration);

        // 위에서부터 화면을 완전히 드러낼 때까지 대기
        yield return _screenTransition.Reveal();

        if(!_screenTransition.Succeeded)
        {
            yield break;
        }

        _isRestarting = false;
    }

    private void ResetBossGauge()
    {
        _killCount = 0;
        SetBossProgress(0f);
    }
}