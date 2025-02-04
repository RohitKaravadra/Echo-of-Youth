using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] SpriteRenderer _Visual;
    [SerializeField] Door _Door;
    // speed

    bool _Pressed = false;

    public Vector2 Position => transform.position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_Pressed)
        {
            _Pressed = true;
            _Door.OnButtonPressed(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_Pressed)
        {
            _Pressed = false;
            _Door.OnButtonPressed(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (_Door != null)
            Gizmos.DrawLine(Position, _Door.transform.position);
    }
}
