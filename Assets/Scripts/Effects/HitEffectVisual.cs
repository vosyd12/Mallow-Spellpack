using UnityEngine;

[ExecuteAlways]
public class HitEffectVisual : MonoBehaviour
{
    private static readonly int HitColorId =
        Shader.PropertyToID("_HitColor");

    private static readonly int HitAmountId =
        Shader.PropertyToID("_HitAmount");

    private static readonly int HitWorldToAreaId =
        Shader.PropertyToID("_HitWorldToArea");

    private static readonly int RimWidthId =
        Shader.PropertyToID("_HitRimWidth");

    private static readonly int CenterStrengthId =
        Shader.PropertyToID("_HitCenterStrength");

    // 몸 얼굴 장식Renderer를 찾을 범위
    [SerializeField, Header("대상")]
    private Transform _visualRoot;

    // 아티스트 기준 피격색 중심 크기
    [SerializeField]
    private Transform _hitArea;

    // Animation이 조절하는 피격 강도
    [Header("Animation에서 조절할 값")]
    [SerializeField, Range(0f, 1f)]
    private float _hitAmount;

    // 피격색
    [SerializeField]
    private Color _hitColor = new Color(1f, 0.4f, 0.4f, 1f);

    // 외곽에서 안쪽으로 색이 퍼지는 범위
    [SerializeField, Range(0.01f, 1f)]
    private float _rimWidth = 0.6f;

    // 중심에 남길 색상 강도
    [SerializeField, Range(0f, 1f)]
    private float _centerStrength = 0.12f;

    private SpriteRenderer[] _renderers;
    private Transform _cachedRoot;
    private MaterialPropertyBlock _propertyBlock;
    private bool _wasVisible;

    private void OnEnable()
    {
        Apply(_hitAmount);
    }

    private void LateUpdate()
    {
        // 피격 중과 효과가 끝나는 순간에만 전달한다.
        if (_hitAmount <= 0f && !_wasVisible)
        {
            return;
        }

        Apply(_hitAmount);
    }

    private bool Prepare()
    {
        if (_visualRoot == null)
        {
            return false;
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        if (_renderers == null || _cachedRoot != _visualRoot)
        {
            _renderers =
                _visualRoot.GetComponentsInChildren<SpriteRenderer>(true);

            _cachedRoot = _visualRoot;
        }

        return true;
    }

    private void Apply(float amount)
    {
        if (!Prepare())
        {
            return;
        }

        amount = Mathf.Clamp01(amount);

        // 영역이 연결되지 않았다면 효과를 표시하지 않는다.
        if (_hitArea == null)
        {
            amount = 0f;
        }

        Matrix4x4 worldToArea = _hitArea != null
            ? _hitArea.worldToLocalMatrix
            : Matrix4x4.identity;

        foreach (SpriteRenderer renderer in _renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            renderer.GetPropertyBlock(_propertyBlock);

            _propertyBlock.SetFloat(HitAmountId, amount);
            _propertyBlock.SetColor(HitColorId, _hitColor);
            _propertyBlock.SetMatrix(HitWorldToAreaId, worldToArea);
            _propertyBlock.SetFloat(
                RimWidthId,
                Mathf.Clamp(_rimWidth, 0.01f, 1f));

            _propertyBlock.SetFloat(
                CenterStrengthId,
                Mathf.Clamp01(_centerStrength));

            renderer.SetPropertyBlock(_propertyBlock);
        }

        _wasVisible = amount > 0f;
    }

    public void ResetEffect()
    {
        _hitAmount = 0f;
        Apply(0f);
    }

    private void OnDisable()
    {
        Apply(0f);
    }

    private void OnDidApplyAnimationProperties()
    {
        if (isActiveAndEnabled)
        {
            Apply(_hitAmount);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_hitArea == null)
        {
            return;
        }

        Matrix4x4 previousMatrix = Gizmos.matrix;
        Color previousColor = Gizmos.color;

        Gizmos.matrix = _hitArea.localToWorldMatrix;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1f, 1f, 0f));

        Gizmos.matrix = previousMatrix;
        Gizmos.color = previousColor;
    }
}