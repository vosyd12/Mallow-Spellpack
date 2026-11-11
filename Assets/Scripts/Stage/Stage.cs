using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int _stageNumber = 1;

    [SerializeField]
    private StageScenery _stageScenery;

    [SerializeField]
    private StageInfo _stageInfo;

    private Player _player;

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

    public void BossGauge()
    {
        // 적이 죽을때 Boss게이지가 조금씩 쌓이는 로직
    }

    private void GameOver()
    {
        // 플레이어가 죽었는지 감지한다.
        if(_player.IsDead)
        {

        }

        // 기본적으로 방치형과 같이 죽었다고 끝인 게임이 아닌
        // 해당 스테이지를 다시 도는 방법으로 처리한다.
        // 단 플레이어의 HP는 리셋
        // 해당 스테이지의 보스게이지 리셋
        // 몬스터 위치도 리셋
        // 즉 플레이어가 죽으면 해당 InGame화면상에 존재하는 수치중 StageNumber를 제외하고 초기화하면된다.
    }
}