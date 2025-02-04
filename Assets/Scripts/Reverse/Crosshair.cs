using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField, Range(1, 100)] int _Range;
    [SerializeField, Range(0.1f, 2)] float _Sensi = 1;
    [SerializeField] Transform _CrosshairObject;
    [SerializeField] Slider _SensiSlider;

    private Camera _MainCam;
    private Animator _Animator;
    private Vector2 _CursorDelta;

    private Vector2 _ScreenCenter;
    private Vector2 _CurserRange = Vector2.zero;

    public bool Enabled { get => gameObject.activeSelf; set { gameObject.SetActive(value); _CrosshairObject.gameObject.SetActive(value); } }
    public Vector2 WorldPos => _MainCam.ScreenToWorldPoint(transform.position);
    public Vector2 LocalPos => _CursorDelta;

    public static Crosshair Instance { get; private set; } // singleton Instance
    public static bool HasInstance => Instance != null;
    private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("Instance of " + this.name + " already exists \n deleting this instance");
            Destroy(this);
        }

        _MainCam = Camera.main;
        _Animator = GetComponent<Animator>();

        _CursorDelta = Vector2.zero;

        _Sensi = PlayerPrefs.GetFloat("Sensi", 0.7f);
        _SensiSlider.value = _Sensi * 100;

        SetBoundsData();
        _SensiSlider.onValueChanged.AddListener((val) =>
        {
            _Sensi = val / 100;
            PlayerPrefs.SetFloat("Sensi", _Sensi);
        });
    }

    private void Start() => OnLook(Vector2.zero);

    private void SetBoundsData()
    {
        _CurserRange = new Vector2(Screen.width, Screen.height) / 200f * _Range;
        _ScreenCenter = new Vector2(Screen.width, Screen.height) / 2;
    }
    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private Vector2 ClampValues(Vector2 value)
    {
        value.x = Mathf.Clamp(value.x, -_CurserRange.x, _CurserRange.x);
        value.y = Mathf.Clamp(value.y, -_CurserRange.y, _CurserRange.y);
        return value;
    }

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
