using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _avoidDistance = 3f;  // 유지할 목표 거리
    [SerializeField] private float _distanceGain = 2f;  // 거리가 어긋났을 때 보정하는 강도
    [SerializeField] private float _orbitSpeed = 2f;    // 적 주위를 도는 속도

    private Vector2 _avoidVelocity;
 
    private PlayerState _player;
    public PlayerState PlayerState => _player;

    private EPlayerJob _job;
    public EPlayerJob Job => _job;

    private Rigidbody2D _rd;

    private Enemy _target;
    public Enemy Target => _target;

    private float _targetSearchTimer;


    private Vector3 _pos;

    private int _levelExp = 100;

    private void Awake()
    {
        _player.LEVEL = 1;
        _player.EXP = 0;
        _player.HP = 100;
        _player.MP = 100;
        _player.STR = 5;
        _player.DEX = 5;
        _player.CON = 5;
        _player.INT = 5;
        _player.WIS = 5;
        _player.LUK = 5;
        _player.AFF = 1;
        _job = EPlayerJob.EWizard;

        _rd = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateTarget();

        // 레벨업 
        if(_player.EXP >= _levelExp)
        {
            _player.LEVEL++;

            int remainingExp = _player.EXP - _levelExp;
            _levelExp = (int)(_levelExp * 1.1f);
            _player.EXP = remainingExp;
        }

        Avoid();
    }

    private void FixedUpdate()
    {
        Vector2 nextPosition =
            _rd.position + _avoidVelocity * Time.fixedDeltaTime;

        _rd.MovePosition(nextPosition);
    }

    private void UpdateTarget()
    {
        if(_enemySpawner == null)
        {
            _target = null;
            return;
        }

        _targetSearchTimer = Time.deltaTime;

        if (_targetSearchTimer > 0f) return;

        _target = _enemySpawner.FindNearestEnemy(_rd.position);
        _targetSearchTimer = 0.2f;
    }

    private void Avoid()
    {
        // 대상이 없을 때 이전 이동이 남지 않도록 초기화한다.
        _avoidVelocity = Vector2.zero;

        if (_target == null || !_target.isActiveAndEnabled) return;

        Vector2 toEnemy = (Vector2)_target.transform.position - _rd.position;

        float maxSpeed = Mathf.Max(0f, _moveSpeed + _player.DEX);
        float distance = toEnemy.magnitude;

        if (distance <= 0.0001f)
        {
            _avoidVelocity = Vector2.right * maxSpeed;
            return;
        }

        Vector2 forward = toEnemy / distance;
        Vector2 right = new Vector2(forward.y, -forward.x);

        float distanceError = distance - _avoidDistance;

        Vector2 correction = forward * distanceError * _distanceGain;

        Vector2 desiredVelocity = right * _orbitSpeed + correction;

        _avoidVelocity = Vector2.ClampMagnitude(desiredVelocity, maxSpeed);
    }
}