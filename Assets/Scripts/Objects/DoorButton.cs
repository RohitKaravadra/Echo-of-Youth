using TMPro;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] SpriteRenderer _Visual;
    [SerializeField] Sprite _Normal;
    [SerializeField] Sprite _Pushed;
    [SerializeField] Door _Door;
    // speed

    bool _Pressed = false;

    public Vector2 Position => transform.position;

    private void Awake()
    {
        if (_Visual != null && _Normal != null)
            _Visual.sprite = _Normal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_Pressed)
        {
            _Pressed = true;
            _Door.OnButtonPressed(true);
            if (_Visual != null && _Pushed != null)
                _Visual.sprite = _Pushed;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_Pressed)
        {
            _Pressed = false;
            _Door.OnButtonPressed(false);
            if (_Visual != null && _Normal != null)
                _Visual.sprite = _Normal;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_Door != null)
            Gizmos.DrawLine(Position, _Door.transform.position);
    }
}
