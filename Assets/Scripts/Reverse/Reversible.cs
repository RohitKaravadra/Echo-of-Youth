
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Reversible : MonoBehaviour, IReversible
{
    [SerializeField] int _MaxReverseSteps = 100;
    [SerializeField] float _SnapshotThreshold = 0.2f;
    [SerializeField] float _TrailThreshold = 0.5f;
    [SerializeField][Range(0, 1)] float _ReverseSpeed = 1;
    [Space(10)]
    [SerializeField] SpriteRenderer _Visuals;
    [SerializeField] GameObject _OutlineObject;
    [SerializeField] LineRenderer _TrailRenderer;

    Rigidbody2D _Rb;
    OutlineEffect _Outline;
    bool _IsReversing = false;

    public Vector2 Position => transform.position;

    struct Snapshot
    {
        public Vector2 pos;
        public Quaternion rot;
        public float time;
        public Snapshot(Vector2 pos, Quaternion rot, float time)
        {
            this.pos = pos;
            this.rot = rot;
            this.time = time;
        }
    }

    CircularQueue<Snapshot> _History;
    float _LastTime;

    bool _Selected;
    bool _Hover;

    private void Awake()
    {
        _Outline = GetComponent<OutlineEffect>();
        _Rb = GetComponent<Rigidbody2D>();

        _Outline.Set(_OutlineObject, _Visuals);
    }

    private void Start()
    {
        _History = new(_MaxReverseSteps);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void SetTrail(bool reverse = false)
    {
        if (_History.Size == 0)
        {
            _TrailRenderer.positionCount = 0;
            return;
        }

        Vector3[] pos = new Vector3[_History.Size];

        int newSize = 0;
        for (int i = 0; i < _History.Size; i++)
        {
            if (i == 0 || Vector2.Distance(_History[i].pos, pos[newSize - 1]) > _TrailThreshold)
                pos[reverse ? _History.Size - newSize++ - 1 : newSize++] = _History[i].pos;
        }

        _TrailRenderer.positionCount = newSize;
        _TrailRenderer.SetPositions(pos);
    }

    private void SetOutline()
    {
        _Outline.Enabled = _Hover || _Selected || _IsReversing;
        _TrailRenderer.enabled = _Hover || _Selected;
    }

    public bool Compare(Transform other) => transform.Equals(other);

    public void OnHover(bool state, bool selected)
    {
        _Hover = state && !selected;
        SetOutline();
    }

    public void OnSelect(bool state)
    {
        _Selected = state;
        SetOutline();

        if (_Selected)
        {
            _History.Clear();
            _LastTime = Time.time;
        }
    }

    public float OnMove(Vector2 position)
    {
        _Rb.MovePosition(Vector2.Lerp(_Rb.position, position, 0.3f));

        if (_History.Size == 0 || (_History.Last.pos - _Rb.position).magnitude > _SnapshotThreshold)
        {
            _History.Enqueue(new Snapshot(transform.position, transform.rotation, Time.time - _LastTime));
            _LastTime = Time.time;
            SetTrail();
        }

        return _History.Size / _History.Capacity;
    }

    public bool OnReverse()
    {
        if (_History.Size == 0)
            return false;

        _Selected = false;
        _Hover = false;

        SetOutline();
        StartCoroutine(Reverse());
        return true;
    }

    IEnumerator Reverse()
    {
        if (_History.Size <= 0)
            yield break;

        _History.Reverse();

        _IsReversing = true;
        SetOutline();
        _Rb.simulated = false;

        Snapshot data = new();
        float delta;

        while (_History.Size > 0)
        {
            data = _History.Dequeue();
            SetTrail();

            while (((Vector2)transform.position - data.pos).magnitude > 0.2f)
            {
                delta = (Time.deltaTime / data.time) * _ReverseSpeed;
                Vector2 pos = Vector2.Lerp(transform.position, data.pos, delta);
                Quaternion rot = Quaternion.Lerp(transform.rotation, data.rot, delta);

                transform.SetPositionAndRotation(pos, rot);

                yield return new WaitForEndOfFrame();
            }

            yield return 0;
        }

        _TrailRenderer.enabled = false;
        _IsReversing = false;
        SetOutline();
        _Rb.simulated = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor"))
            ReverseGun.OnObjectHover?.Invoke(transform, true);

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor"))
            ReverseGun.OnObjectHover?.Invoke(transform, false);
    }
}
