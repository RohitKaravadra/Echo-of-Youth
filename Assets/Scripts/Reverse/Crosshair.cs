using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField, Range(1, 100)] int _Range;
    [SerializeField, Range(0.1f, 1)] float _Sensi = 10;
    [SerializeField] Transform _CrosshairObject;

    private Camera _MainCam;
    private Animator _Animator;
    private Vector2 _CursorDelta;

    private Vector2 _ScreenCenter;
    private float _CurserRange = 0;

    public bool Enabled { get => gameObject.activeSelf; set { gameObject.SetActive(value); _CrosshairObject.gameObject.SetActive(value); } }
    public Vector2 WorldPos => _MainCam.ScreenToWorldPoint(transform.position);
    public Vector2 LocalPos => _CursorDelta;

    public static Crosshair Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        _MainCam = Camera.main;
        _Animator = GetComponent<Animator>();

        _CursorDelta = Vector2.zero;
        SetBoundsData();
    }

    private void Start() => OnLook(Vector2.zero);

    private void SetBoundsData()
    {
        _CurserRange = Screen.height / 200f * _Range;
        _ScreenCenter = new Vector2(Screen.width, Screen.height) / 2;
    }
    private void UpdateSensi(float value) => _Sensi = value;

    private Vector2 ClampValues(Vector2 value) => Vector2.ClampMagnitude(value, _CurserRange);

    public void OnLook(Vector2 value)
    {
        _CursorDelta = ClampValues(_CursorDelta + value * _Sensi);
        transform.position = _ScreenCenter + _CursorDelta;
        if (_CrosshairObject != null)
            _CrosshairObject.position = WorldPos;
    }

    public void OnHit() => _Animator.SetTrigger("Hit");

    private void OnRectTransformDimensionsChange() => SetBoundsData();
}
