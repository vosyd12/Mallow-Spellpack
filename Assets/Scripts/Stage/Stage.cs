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


    [SerializeField, Min(0.1f)]
    private float _bossIntroDuration = 1f;

    [SerializeField, Range(0.2f, 1f)]
    private float _bossZoomFactor = 0.5f;

    [SerializeField]
    private AnimationCurve _bossIntroCurve;

    [SerializeField]
    private float _bossFightDuration = 15f;

    private float _bossFightEndsAt = -1f;

    private bool _isRestarting;

    public bool IsRestarting => _isRestarting;

    private int _killCount;

    private int _stageMaxKill = 100;

    private float _bossProgress;

    private Enemy _activeBoss;

    public int StageNumber => _stageNumber;

    private bool _isBossIntroPlaying;

    private bool CanContinueStage =>
        _player != null && !_player.IsDead && !_isRestarting;

    public bool CanSpawnNormalEnemies =>
        CanContinueStage && !_isBossIntroPlaying && _activeBoss == null;

    public bool CanFight =>
        CanContinueStage && !_isBossIntroPlaying &&
        (_activeBoss == null || !_activeBoss.IsDead);

    private void Start()
    {
        EnterStage(_stageNumber);
    }

    private void LateUpdate()
    {
        // 게이지 만충과 플레이어 사망이 겹치면 재시작을 우선한다.
        GameOver();
        UpdateBossTimer();

        if(!CanFight)
        {
            return;
        }

        // 마지막 적 처지로 레벨업과 보스 조건이 겹치면 카드를 먼저 연다.
        _player.TryLevelUp();

        if(_player.IsLevelUpCardOpen)
        {
            return;
        }

        if (_killCount >= _killsForBoss && 
            CanSpawnNormalEnemies)
        {
            TryStartBossIntro();
        }
    }

    private void TryStartBossIntro()
    {
        Camera camera = Camera.main;
        if (camera == null || 
           _enemySpawner == null ||
           !_enemySpawner.TrySpawnBoss(out Enemy boss))
        {
            return;
        }

        _activeBoss = boss;
        _isBossIntroPlaying = true;
        boss.HealthChanged += RefreshStageInfo;
        RefreshStageInfo();

        ClearBullets();
        _stageInfo?.ShowBossIntro(Mathf.CeilToInt(_bossFightDuration));

        StartCoroutine(BossIntroRoutine(camera, boss.transform.position));
    }

    private IEnumerator BossIntroRoutine(Camera camera, Vector3 bossPosition)
    {
        Vector3 originalPosition = camera.transform.position;
        float originalSize = camera.orthographicSize;
        bossPosition.z = originalPosition.z;

        bool resumeFollow =
            camera.TryGetComponent<PlayerCameraFollow>(out var follow) &&
            follow.enabled;

        if(resumeFollow)
        {
            follow.enabled = false;
        }

        try
        {
            float elapsed = 0f;
            while (elapsed < _bossIntroDuration && 
                CanContinueStage)
            {
                float progress = 
                    elapsed / _bossIntroDuration;

                float weight = _bossIntroCurve != null && _bossIntroCurve.length > 0
                    ? Mathf.Clamp01(_bossIntroCurve.Evaluate(progress))
                    : Mathf.SmoothStep(0f, 1f, Mathf.PingPong(progress * 2f, 1f));

                camera.transform.position =
                    Vector3.Lerp(originalPosition, bossPosition, weight);

                camera.orthographicSize =
                    Mathf.Lerp(originalSize, originalSize * _bossZoomFactor, weight);

                yield return null;
                elapsed += Time.unscaledDeltaTime;
            }
        }

        finally
        {
            // 연출 종료나 재시작 시 원래 전투 구도를 복구한다.
            camera.transform.position = originalPosition;
            camera.orthographicSize = originalSize;
            _isBossIntroPlaying = false;

            // 카메라 복귀 후 숫자 카운트다운과 모래시계를 함께 시작한다.
            if (CanFight && _activeBoss != null)
            {
                _bossFightEndsAt = Time.time + _bossFightDuration;
                _stageInfo?.StartBossTimerAnimation();
            }

            if(resumeFollow)
            {
                follow.enabled = true;
            }
        }
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

        if (_activeBoss != null)
        {
            _activeBoss.HealthChanged -= RefreshStageInfo;
        }

        _stageNumber = stageNumber;
        _activeBoss = null;
        ResetBossPresentation();

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
        if (_stageInfo == null)
        {
            return;
        }

        bool showingBossHealth = _activeBoss != null;
        float gaugeRatio = showingBossHealth ? _activeBoss.HealthRatio : _bossProgress;

        _stageInfo.SetInfo(_stageNumber, gaugeRatio);
    }

    public void GameOver()
    {
        if(_player == null ||
            !_player.IsDead)
        {
            return;
        }

        BeginStageRestart();
    }

    private void BeginStageRestart()
    {
        if(_isRestarting)
        {
            return;
        }

        if (_screenTransition == null ||
            _enemySpawner == null ||
            _playerSpawnPoint == null)
        {
            Debug.LogError("스테이지 재시작에 필요한 참조를 확인하세요.", this);
            return;
        }

        // 화면 전환을 시작하기 전에 전투와 중복 재시작을 막는다.
        _isRestarting = true;
        ResetBossPresentation();
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
        Bullet[] bullets =
            FindObjectsByType<Bullet>(FindObjectsInactive.Include);

        foreach (Bullet bullet in bullets)
        {
            bullet.gameObject.SetActive(false);
            Destroy(bullet.gameObject);
        }
    }

    public void OnBossDeathMotionFinished(Enemy enemy)
    {
        if (!CanContinueStage || 
            enemy == null ||
            enemy != _activeBoss || 
            !enemy.IsDead)
        {
            return;
        }

        ClearBullets();
        CompleteStage();
    }


    private void UpdateBossTimer()
    {
        if (_bossFightEndsAt < 0f)
            return;

        if (!CanFight || _activeBoss == null)
        {
            ResetBossPresentation();
            return;
        }

        float remaining = _bossFightEndsAt - Time.time;

        if(remaining <= 0f)
        {
            // 타이머가 만료되면 같은 스테이지의 일반 전투로 돌아간다.
            BeginStageRestart();
            return;
        }

        _stageInfo?.SetBossTimerSeconds(Mathf.CeilToInt(remaining));
    }

    private void ResetBossPresentation()
    {
        _bossFightEndsAt = -1f;
        _stageInfo?.HideBossPresentation();
    }
}