using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fill;       // 실질적인 바 게이지
    [SerializeField] private Image _damageFill; // 잔상에 대한 바게이지

    [SerializeField]
    private GameObject _visualRoot;

    [SerializeField]
    private float _damageDelay = 0.2f;

    [SerializeField]    // 데미지 잔상바가 사라지는 속도
    private float _damageSpeed = 1f;

    private float _delayTimer;
    private bool _isInitialized;

    private void Update()
    {
        if(!_isInitialized ||
            _damageFill.fillAmount <= _fill.fillAmount)
        {
            return;
        }

        if(_delayTimer > 0f)
        {
            _delayTimer -= Time.deltaTime;
            return;
        }

        _damageFill.fillAmount = Mathf.MoveTowards(
            _damageFill.fillAmount,
            _fill.fillAmount,
            _damageSpeed * Time.deltaTime);
    }

    public void SetHealth(int currentHP, int maxHP)
    {
        float ratio = maxHP > 0
            ? (float)currentHP / maxHP
            : 0f;

        float previousRatio = _fill.fillAmount;
        _fill.fillAmount = ratio;

        // 최초 표시와 회복은 잔상도 즉시 맞춘다
        if (!_isInitialized || ratio > previousRatio)
        {
            _damageFill.fillAmount = ratio;
            _delayTimer = 0f;
            _isInitialized = true;
            return;
        }

        // 연속 피격 시 잔상의 대기 시간을 갱신한다.
        if (ratio < previousRatio)
        {
            _delayTimer = _damageDelay;
        }
    }

    public void Hide()
    {
        _visualRoot.SetActive(false);
        enabled = false;
    }

    private void OnDisable()
    {
        _isInitialized = false;
        _delayTimer = 0f;
    }
}