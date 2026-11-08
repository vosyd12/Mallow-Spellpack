using UnityEngine;

public class HitReaction : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _bodyRenderer;

    [SerializeField]
    private Color _hitColor = new Color(1f, 0.4f, 0.4f, 1f);

    [SerializeField, Min(0.01f)]
    private float _duration = 0.12f;

    private Color _originalColor;
    private float _remainingTime;

    private void Awake()
    {
        _originalColor = _bodyRenderer.color;
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
            _bodyRenderer.color = _originalColor;
        }
    }

    public void Play()
    {
        _remainingTime = _duration;
        _bodyRenderer.color = _hitColor;
    }

    private void OnDisable()
    {
        _remainingTime = 0f;

        if (_bodyRenderer != null)
        {
            _bodyRenderer.color = _originalColor;
        }
    }
}
