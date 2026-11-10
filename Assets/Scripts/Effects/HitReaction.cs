using UnityEngine;

public class HitReaction : MonoBehaviour
{
    // 셰이더 프로퍼티 공통 피격 색상
    private static readonly int HitColorId =
        Shader.PropertyToID("_HitColor");

    // 셰이더 프로퍼티 피격 색으로 덮는 강도
    private static readonly int HitAmountId =
        Shader.PropertyToID("_HitAmount");

    [SerializeField]
    private SpriteRenderer _bodyRenderer;

    [SerializeField]
    private Color _hitColor = new Color(1f, 0.4f, 0.4f, 1f);

    [SerializeField, Min(0.01f)]
    private float _duration = 0.12f;

    private MaterialPropertyBlock _propertyBlock;
    private float _remainingTime;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();

        SetHitAmount(0f);
    }

    private void Update()
    {
        if(_remainingTime <= 0f)
        {
            return;
        }

        _remainingTime = Mathf.Max(
            0f,
            _remainingTime - Time.deltaTime);

        if(_remainingTime <= 0f)
        {
            SetHitAmount(0f);
        }
    }

    public void Play()
    {
        // 연속으로 맞으면 피격 표시 시간을 다시 시작
        _remainingTime = _duration;
        SetHitAmount(1f);
    }

    private void SetHitAmount(float amount)
    {
        if (_bodyRenderer == null ||
            _propertyBlock == null)
        {
            return;
        }

        // 다른 곳에서 설정한 프로퍼티는 유지
        _bodyRenderer.GetPropertyBlock(_propertyBlock);

        // 뒤에 있는 값을 앞에 변수에 복사한다.
        _propertyBlock.SetColor(HitColorId, _hitColor);
        _propertyBlock.SetFloat(HitAmountId, amount);

        // 공유 Material 대신 Renderer에만 적용
        _bodyRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void OnDisable()
    {
        _remainingTime = 0f;

        SetHitAmount(0f);
    }
}
