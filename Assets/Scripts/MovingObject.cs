
using UnityEngine;

public class MovingObject : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer _Visual;
    [SerializeField] float _Speed;
    [SerializeField][Range(-1, 1)] int _Direction = 1;
    [SerializeField] float _MinThreshold;
    [SerializeField] Vector2[] _MovePositions;
    // speed

    OutlineEffect _Outline;
    Rigidbody2D _Rigidbody;

    float _LastReverse = 0;
    int _CurrentIndex;

    bool _Hover = false;
    Vector2 _InitPosition; // initial position of this object
    public Vector2 Position => transform.position;
    private bool CanReverse => Time.time - _LastReverse > _MinThreshold;

    private void Start()
    {
        _InitPosition = transform.position;
        _Outline.Set(_Visual);
    }

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody2D>();
        _Outline = GetComponent<OutlineEffect>();
        _LastReverse = Time.time;
        _CurrentIndex = 0;
    }

    private void Update()
    {
        _Outline.Enabled = _Hover && CanReverse;

        if (Vector2.Distance(transform.position, _InitPosition + _MovePositions[_CurrentIndex]) < 0.1f)
        {
            _Rigidbody.MovePosition(_InitPosition + _MovePositions[_CurrentIndex]);
            UpdateIndex();
            return;
        }
    }

    private void FixedUpdate()
    {
        _Rigidbody.linearVelocity =
            (_InitPosition + _MovePositions[_CurrentIndex] - (Vector2)transform.position).normalized
            * Time.deltaTime * _Speed;
    }

    private void UpdateIndex()
    {
        _CurrentIndex = (_CurrentIndex + _Direction) % _MovePositions.Length;
        if (_CurrentIndex < 0) _CurrentIndex = _CurrentIndex + _MovePositions.Length;
    }

    public bool Compare(Transform other) => transform.Equals(other.transform);

    public void OnHover(bool state, bool selected = false)
    {
        print("Hover");
        _Hover = state;
    }

    public void OnMove(Vector2 position)
    {
        // empty
    }

    public bool OnReverse()
    {
        if (!CanReverse)
            return false;

        _LastReverse = Time.time;
        _Direction *= -1;
        UpdateIndex();

        return _MinThreshold != 0;
    }

    public void OnSelect(bool state)
    {

    }

    private void OnDrawGizmos()
    {
        if (_MovePositions == null || _MovePositions.Length == 0)
            return;

        Vector2 initPos = _InitPosition == Vector2.zero ? transform.position : _InitPosition;

        // draw path through all points
        for (int i = 0; i < _MovePositions.Length - 1; i++)
        {
            Gizmos.color = i == 0 ? Color.yellow : Color.green;
            Gizmos.DrawSphere(initPos + _MovePositions[i], 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(initPos + _MovePositions[i], initPos + _MovePositions[i + 1]);
        }

        // connect end with start
        Gizmos.DrawLine(initPos + _MovePositions[_MovePositions.Length - 1], initPos + _MovePositions[0]);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(initPos + _MovePositions[_MovePositions.Length - 1], 0.1f);
    }
}
