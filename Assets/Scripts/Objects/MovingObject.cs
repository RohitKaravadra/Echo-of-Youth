using System.Collections;
using UnityEngine;

public class MovingObject : MonoBehaviour, IInteractable
{
    [SerializeField] SpriteRenderer _Visual;
    [SerializeField] float _Speed;
    [Space(5)]
    [SerializeField] Vector2 _EndPos;
    [SerializeField] MovingObject _Connection;
    // speed

    OutlineEffect _Outline;
    Rigidbody2D _Rigidbody;

    int _CurrentIndex;

    bool _Hover = false;
    Vector2 _InitPosition; // initial position of this object
    public Vector2 Position => transform.position;
    private bool _IsMoving;

    private void Start()
    {
        _InitPosition = transform.position;
        _Outline.Set(_Visual);
    }

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody2D>();
        _Outline = GetComponent<OutlineEffect>();
        _CurrentIndex = 0;
    }

    private void OnDestroy() => StopCoroutine(nameof(StartMoving));
    private void Update() => _Outline.Enabled = _Hover && !_IsMoving;
    public bool Compare(Transform other) => transform.Equals(other.transform);

    public void OnHover(bool state, bool selected = false) => _Hover = state && !selected;

    public void OnMove(Vector2 position)
    {
        // empty
    }

    public bool OnReverse()
    {
        if (_IsMoving)
            return false;

        StartCoroutine(nameof(StartMoving));

        if (_Connection != null)
            _Connection.OnReverse();

        return true;
    }

    public void OnSelect(bool state)
    {

    }

    IEnumerator StartMoving()
    {
        _IsMoving = true;

        Vector2 startPos = _InitPosition;
        Vector2 endPos = _InitPosition + _EndPos;

        Vector2 dir = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, endPos);

        while (Vector2.Distance(transform.position, startPos) < distance)
        {
            _Rigidbody.MovePosition((Vector2)transform.position + _Speed * Time.fixedDeltaTime * dir);
            yield return new WaitForFixedUpdate();

        }

        dir = (startPos - endPos).normalized;
        while (Vector2.Distance(transform.position, endPos) < distance)
        {
            _Rigidbody.MovePosition((Vector2)transform.position + _Speed * Time.fixedDeltaTime * dir);
            yield return new WaitForFixedUpdate();
        }

        transform.position = startPos;
        _IsMoving = false;

        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 pos = _InitPosition == Vector2.zero ? transform.position : _InitPosition;
        Gizmos.DrawLine(pos, pos + _EndPos);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pos + _EndPos, 0.1f);

        if (_Connection != null)
            Gizmos.DrawLine(transform.position, _Connection.Position);
    }
}
