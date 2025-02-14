using System;
using UnityEngine;

public class ReverseGun : MonoBehaviour
{
    [SerializeField] float _MinGunPullDistance;
    [SerializeField] float _MaxObjectDistance;
    [SerializeField] Transform _GunHead;
    [SerializeField] Laser _Laser;
    [SerializeField] ShakeData _CameraShake;

    bool _Selected = false;

    IInteractable _HoveringObject = null;
    IInteractable _SelectedObject = null;

    Vector2 _CursorPos = Vector2.zero;
    bool _TooClose = false;

    public bool Enabled
    {
        get => gameObject.activeSelf; set
        {
            gameObject.SetActive(value);
            if (Crosshair.HasInstance)
                Crosshair.Instance.Enabled = value;

            _HoveringObject = null;
            OnSelected(false);
        }
    }

    private void OnEnable()
    {
        GameEvents.Input.OnObjectSelect += OnSelected;
        GameEvents.Input.OnObjectReverse += OnReverse;
        GameEvents.Input.OnPlayerLook += OnLook;
        GameEvents.Game.OnPlayerDead += ResetGun;
        CrosshairCollision.OnObjectHover += OnHover;
    }

    private void OnDisable()
    {
        GameEvents.Input.OnObjectSelect -= OnSelected;
        GameEvents.Input.OnObjectReverse -= OnReverse;
        GameEvents.Input.OnPlayerLook -= OnLook;
        GameEvents.Game.OnPlayerDead -= ResetGun;
        CrosshairCollision.OnObjectHover -= OnHover;
    }

    private void Start()
    {
        Crosshair.Instance.enabled = Enabled;
    }

    private void Update()
    {
        _CursorPos = Crosshair.Instance.WorldPos;
        _TooClose = Vector2.Distance(transform.position, _CursorPos) < 0.2f;

        SetRotation();
        CheckDrag();
    }

    private void OnLook(Vector2 val) => Crosshair.Instance?.OnLook(val);

    private void CheckDrag()
    {
        if (_Laser.Enabled)
        {
            CameraManager.Instance?.ApplyShake(_CameraShake);

            Vector2 myPos = transform.position;
            Vector2 diff = _CursorPos - myPos;

            if (_TooClose)
            {
                _Laser.Set(_GunHead.position, transform.position + transform.up * _MinGunPullDistance);
                return;
            }

            Vector2 curPos = diff.magnitude < _MinGunPullDistance ?
                 myPos + diff.normalized * _MinGunPullDistance : _CursorPos;

            if (_SelectedObject != null)
            {
                // check if object is opposite side of player body
                if (Vector2.Dot(curPos - myPos, _SelectedObject.Position - myPos) < 0.2f)
                {
                    _SelectedObject?.OnSelect(false);
                    _SelectedObject = null;
                    _HoveringObject?.OnHover(true);
                    return;
                }

                if (Vector2.Distance(_SelectedObject.Position, curPos) > _MaxObjectDistance)
                {
                    _SelectedObject?.OnSelect(false);
                    _SelectedObject = null;
                }
                else
                {
                    _Laser.Set(_GunHead.position, _SelectedObject.Position);
                    _SelectedObject.OnMove(curPos);
                }
            }
            else
                _Laser.Set(_GunHead.position, curPos);
        }
    }

    private void SetRotation()
    {
        if (_TooClose)
            return;

        transform.localScale = new Vector3(transform.position.x < _CursorPos.x ? 1 : -1, 1, 1);
        transform.up = (_CursorPos - (Vector2)transform.position).normalized;
    }

    private void OnHover(Transform obj, bool state)
    {
        if (state)
        {
            if (obj.TryGetComponent(out IInteractable newHover))
            {
                _HoveringObject?.OnHover(false);
                _HoveringObject = newHover;
                _HoveringObject.OnHover(state && !_Selected);

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
            AudioManager.Instance?.PlayLaser();
        }
        else
        {
            _SelectedObject?.OnSelect(false);
            _SelectedObject = null;
            _HoveringObject?.OnHover(true);
            AudioManager.Instance?.StopLaser();
        }

        _Laser.Enabled = state;
    }

    private void OnReverse(bool state)
    {
        if (state && _HoveringObject != null)
        {
            if (_HoveringObject.OnReverse())
            {
                AudioManager.Instance?.PlaySound(AudioFile.Reverse);
                _SelectedObject = null;
                _HoveringObject.OnHover(false);
                _HoveringObject = null;
            }
        }
    }

    private void ResetGun()
    {
        OnSelected(false);
        OnHover(null, false);
    }
}
