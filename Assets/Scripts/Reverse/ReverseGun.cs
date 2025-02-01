using System;
using UnityEngine;

public class ReverseGun : MonoBehaviour
{
    [SerializeField] float _MinGunPullDistance;
    public static Action<Transform, bool> OnObjectHover;

    IReversible _Hovering = null;
    IReversible _Selected = null;

    Vector2 _CursorPos = Vector2.zero;

    public bool Enabled
    {
        get => gameObject.activeSelf; set
        {
            gameObject.SetActive(value);
            if (Crosshair.Instance != null) Crosshair.Instance.Enable = value;
        }
    }

    private void OnEnable()
    {
        GameEvents.Input.OnObjectSelect += OnSelected;
        GameEvents.Input.OnObjectReverse += OnReverse;
        OnObjectHover += ObjectHover;
    }

    private void OnDisable()
    {
        GameEvents.Input.OnObjectSelect -= OnSelected;
        GameEvents.Input.OnObjectReverse -= OnReverse;
        OnObjectHover -= ObjectHover;
        if (Crosshair.Instance != null)
            Crosshair.Instance.Enable = false;
    }

    private void Start()
    {
        Crosshair.Instance.enabled = Enabled;
    }

    private void Update()
    {
        _CursorPos = Crosshair.Instance.WorldPos;
        SetRotation();
        CheckDrag();
    }

    private void CheckDrag()
    {
        Vector2 pos = transform.position;
        Vector2 diff = _CursorPos - pos;

        pos = diff.magnitude < _MinGunPullDistance ?
             pos + diff.normalized * _MinGunPullDistance : _CursorPos;

        _Selected?.OnMove(pos);
    }

    private void SetRotation()
    {
        transform.localScale = new Vector3(transform.position.x < _CursorPos.x ? 1 : -1, 1, 1);
        transform.up = (_CursorPos - (Vector2)transform.position).normalized;
    }

    private void ObjectHover(Transform obj, bool state)
    {
        if (_Selected != null)
            return;

        if (state)
        {
            _Hovering?.OnHover(false);

            if (obj.TryGetComponent<IReversible>(out _Hovering))
                _Hovering.OnHover(state);
            else
                _Hovering = null;
        }
        else
        {
            if (_Hovering != null && _Hovering.Compare(obj))
            {
                _Hovering.OnHover(false);
                _Hovering = null;
            }
        }
    }

    private void OnSelected(bool state)
    {
        if (state)
        {
            if (_Selected == null && _Hovering != null)
            {
                _Selected = _Hovering;
                _Selected.OnSelect(true);
            }
        }
        else
        {
            if (_Selected != null)
            {
                _Selected.OnSelect(false);
                _Selected = null;
            }
        }
    }


    private void OnReverse(bool state)
    {
        if (state && _Hovering != null)
        {
            if (_Hovering.OnReverse())
            {
                _Selected = null;
                _Hovering = null;
            }
        }
    }
}
