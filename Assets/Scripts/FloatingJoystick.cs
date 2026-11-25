using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

public class FloatingJoystick : OnScreenControl,
    IPointerDownHandler,    // 누른 위치에 조이스틱을 표시
    IDragHandler,           // 드래그 방향과 거리를 이동 입력으로 보낸다.
    IPointerUpHandler       // 이동 입력을 0으로 돌리고 조이스틱을 숨긴다.
{
    [SerializeField]
    private string _controlPath = "<Gamepad>/leftStick";

    [SerializeField]
    private RectTransform _visualRoot;

    [SerializeField]
    private RectTransform _handle;

    [SerializeField, Min(1f)]
    private float _movementRange = 47f;

    private RectTransform _touchArea;
    private Vector2 _pressPosition;
    private bool _isDragging;
    private int _activePointerId;

    // OnScreenControl이 이 경로로 가상 게임패드의 왼쪽 스틱을 찾는다.
    protected override string controlPathInternal
    {
        get => _controlPath;
        set => _controlPath = value;
    }


    private void Awake()
    {
        _touchArea = GetComponent<RectTransform>();

        // 감지 영역은 활성 상태로 두고, 원판과 손잡이만 숨긴다.
        if (_visualRoot != null)
        {
            _visualRoot.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isDragging ||
            eventData.button != PointerEventData.InputButton.Left ||
            _visualRoot == null ||
            _handle == null ||
            !TryGetLocalPoint(eventData, out Vector2 point))
        {
            return;
        }

        // 처음 누른 손가락과 위치를 기억한다. 이 위치가 스틱의 원점이 된다.
        _activePointerId = eventData.pointerId;
        _pressPosition = point;
        _isDragging = true;

        // 원판은 감지 영역의 자식이므로 부모의 로컬 좌표를 사용한다.
        _visualRoot.localPosition = new Vector3(
            point.x, point.y, _visualRoot.localPosition.z);

        _handle.anchoredPosition = Vector2.zero;
        _visualRoot.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging ||
            _activePointerId != eventData.pointerId ||
            !TryGetLocalPoint(eventData, out Vector2 point))
        {
            return;
        }

        // 손가락이 더 멀리 가도 손잡이는 최대 이동 반경 안에 둔다.
        Vector2 offset = Vector2.ClampMagnitude(
            point - _pressPosition, _movementRange);

        _handle.anchoredPosition = offset;

        // 픽셀 대신 크기가 0 ~ 1인 방향 벡터를 Player/Move에 전달한다.
        SendValueToControl(offset / _movementRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging && 
            _activePointerId == eventData.pointerId)
        {
            ReleaseStick();
        }
    }

    protected override void OnDisable()
    {
        ReleaseStick();
        base.OnDisable();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            ReleaseStick();
        }
    }

    private bool TryGetLocalPoint(
        PointerEventData eventData, out Vector2 point)
    {
        // 화면 좌표를 감지 영역 좌표로 바꿔 Canvas 배율을 반영한다.
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _touchArea, eventData.position, eventData.pressEventCamera,
            out point);
    }

    private void ReleaseStick()
    {
        if (!_isDragging)
        {
            return;
        }

        // 손을 떼거나 포커스를 잃으면 이동 값을 0으로 되돌린다.
        _isDragging = false;
        SendValueToControl(Vector2.zero);
        _handle.anchoredPosition = Vector2.zero;
        _visualRoot.gameObject.SetActive(false);
    }
}