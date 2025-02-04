using System;
using System.Linq;
using UnityEngine;

public class MovingObject : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer _Visual;
    [SerializeField] Vector2[] _MovePositions;
    [SerializeField] float _Speed;
    [SerializeField] float _MinThreshold;
    // speed

    OutlineEffect _Outline;
    Rigidbody2D _Rigidbody;
    float _LastReverse = 0;
    int _CurrentIndex;
    bool _Hover = false;
    Vector2 _InitPosition; // initial position of this object
    public Vector2 Position => transform.position;

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
        if (Vector2.Distance(transform.position, _InitPosition + _MovePositions[_CurrentIndex]) < 0.1f)
            _CurrentIndex = (_CurrentIndex + 1) % _MovePositions.Length;

        Vector2 pos = Vector2.MoveTowards(transform.position, _InitPosition + _MovePositions[_CurrentIndex], Time.deltaTime * _Speed);
        _Rigidbody.MovePosition(pos);
    }

    public bool Compare(Transform other)
    {
        return false;
    }

    public void OnHover(bool state, bool selected = false)
    {
        print("Hover");
        _Hover = state;
        _Outline.Enabled = _Hover;
    }

    public void OnMove(Vector2 position)
    {
        // empty
    }

    public bool OnReverse()
    {
        if (Time.time - _LastReverse < _MinThreshold)
            return false;

        _LastReverse = Time.time;
        ReversePositions();

        return true;
    }

    public void OnSelect(bool state)
    {

    }

    private void ReversePositions()
    {
        Array.Reverse(_MovePositions, 0, _MovePositions.Length);
        _CurrentIndex = _MovePositions.Length - _CurrentIndex - 1;
    }

    private void OnDrawGizmos()
    {
        if (_MovePositions.Length == 0)
            return;

        Vector2 initPos = _InitPosition == Vector2.zero ? transform.position : _InitPosition;

        Gizmos.color = Color.green;
        for (int i = 0; i < _MovePositions.Length - 1; i++)
        {
            Gizmos.DrawSphere(initPos + _MovePositions[i], 0.1f);
            Gizmos.DrawLine(initPos + _MovePositions[i], initPos + _MovePositions[i + 1]);
        }
        Gizmos.DrawSphere(initPos + _MovePositions.Last(), 0.1f);
    }
}
