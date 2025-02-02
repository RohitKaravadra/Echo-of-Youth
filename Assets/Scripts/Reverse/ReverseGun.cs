using System;
using UnityEngine;

public class ReverseGun : MonoBehaviour
{
    [SerializeField] float _MinGunPullDistance;
    [SerializeField] Transform _GunHead;
    [SerializeField] Laser _Laser;

    public static Action<Transform, bool> OnObjectHover;

    bool _Selected = false;

    IReversible _HoveringObject = null;
    IReversible _SelectedObject = null;

    Vector2 _CursorPos = Vector2.zero;

    public bool Enabled
    {
        get => gameObject.activeSelf; set
        {
            gameObject.SetActive(value);
            if (Crosshair.Instance != null)
                Crosshair.Instance.Enabled = value;
            _Laser.Enabled = false;
        }
    }

    private void OnEnable()
    {
        GameEvents.Input.OnObjectSelect += OnSelected;
        GameEvents.Input.OnObjectReverse += OnReverse;
        GameEvents.Input.OnPlayerLook += OnLook;
        OnObjectHover += ObjectHover;
    }

    private void OnDisable()
    {
        GameEvents.Input.OnObjectSelect -= OnSelected;
        GameEvents.Input.OnObjectReverse -= OnReverse;
        GameEvents.Input.OnPlayerLook -= OnLook;
        OnObjectHover -= ObjectHover;
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

    private void OnLook(Vector2 val) => Crosshair.Instance?.OnLook(val);

    private void CheckDrag()
    {
        if (_Laser.Enabled)
        {
            Vector2 pos = transform.position;
            Vector2 diff = _CursorPos - pos;

            pos = diff.magnitude < _MinGunPullDistance ?
                 pos + diff.normalized * _MinGunPullDistance : _CursorPos;

            if (_SelectedObject != null)
            {
                _Laser.Set(_GunHead.position, _SelectedObject.Position);
                _SelectedObject.OnMove(pos);
            }
            else
                _Laser.Set(_GunHead.position, pos);
        }
    }

    private void SetRotation()
    {
        transform.localScale = new Vector3(transform.position.x < _CursorPos.x ? 1 : -1, 1, 1);
        transform.up = (_CursorPos - (Vector2)transform.position).normalized;
    }

    private void ObjectHover(Transform obj, bool state)
    {
        if (state)
        {
            if (obj.TryGetComponent<IReversible>(out IReversible newHover))
            {
                _HoveringObject?.OnHover(false);
                _HoveringObject = newHover;
                _HoveringObject.OnHover(state, _Selected);

                if (_Selected && _SelectedObject == null)
                {
                    _SelectedObject = _HoveringObject;
                    _SelectedObject.OnSelect(true);
                }
            }
        }
        else
        {
            if (_HoveringObject != null && _HoveringObject.Compare(obj))
            {
                _HoveringObject.OnHover(false);
                _HoveringObject = null;
            }
        }
    }

    private void OnSelected(bool state)
    {
        _Selected = state;

        if (state)
        {
            if (_SelectedObject == null && _HoveringObject != null)
            {
                _SelectedObject = _HoveringObject;
                _SelectedObject.OnSelect(true);
            }
        }
        else
        {
            _SelectedObject?.OnSelect(false);
            _SelectedObject = null;
            _HoveringObject?.OnHover(true, _Selected);
        }

        _Laser.Enabled = state;
    }

    private void OnReverse(bool state)
    {
        if (state && _HoveringObject != null)
        {
            if (_HoveringObject.OnReverse())
            {
                _SelectedObject = null;
                _HoveringObject = null;
            }
        }
    }
}
