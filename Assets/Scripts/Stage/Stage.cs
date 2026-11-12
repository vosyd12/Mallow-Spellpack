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

    [SerializeField]
    private EnemySpawner _enemySpawner;

    [SerializeField]
    private Transform _playerSpawnPoint;

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

        if (_screenTransition == null ||
            _enemySpawner == null ||
            _playerSpawnPoint == null)
        {
            Debug.LogError("스테이지 재시작 참조를 확인해주세요.", this);
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
        yield return _screenTransition.Cover();

        if (!_screenTransition.Succeeded)
        {
            yield break;
        }

        ResetCurrentStage();

        yield return new WaitForSecondsRealtime(_blackHoldDuration);

        yield return _screenTransition.Reveal();

        if (!_screenTransition.Succeeded)
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

    private void ResetCurrentStage()
    {
        // 기존 전투 정리
        ClearBullets();
        _enemySpawner.ClearEnemies();

        if (LandingDustPool.Instance != null)
        {
            LandingDustPool.Instance.ClearActiveEffects();
        }

        // 같은 스테이지의 배경·처치 수·보스 게이지 재구성
        EnterStage(_stageNumber);

        // 성장 데이터는 유지하고 위치·HP·행동 상태 복구
        _player.Revive(_playerSpawnPoint.position);
    }

    private void ClearBullets()
    {
        Bullet[] bullets = FindObjectsByType<Bullet>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        foreach (Bullet bullet in bullets)
        {
            bullet.gameObject.SetActive(false);
            Destroy(bullet.gameObject);
        }
    }
}