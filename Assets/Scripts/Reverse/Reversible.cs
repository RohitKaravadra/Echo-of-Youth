
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Reversible : MonoBehaviour, IReversible
{
    [SerializeField] int _MaxReverseSteps;
    [SerializeField] float _MinSnapshotDistance;
    [Space(10)]
    [SerializeField] SpriteRenderer _Visuals;
    [SerializeField] GameObject _OutlineObject;

    Rigidbody2D _Rb;
    OutlineEffect _Outline;

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

    CircularQueue<Snapshot> _ReverseData;
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
        _ReverseData = new(_MaxReverseSteps);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void SetOutline() => _Outline.EnableOutline = _Hover || _Selected;

    public bool Compare(Transform other) => transform.Equals(other);

    public void OnHover(bool state)
    {
        _Hover = state;
        SetOutline();
    }

    public void OnSelect(bool state)
    {
        _Selected = state;
        SetOutline();

        if (_Selected)
        {
            _ReverseData.Clear();
            _LastTime = Time.time;
        }
    }

    public float OnMove(Vector2 position)
    {
        _Rb.MovePosition(Vector2.Lerp(_Rb.position, position, 0.3f));

        if (_ReverseData.Size == 0 || (_ReverseData.Last.pos - _Rb.position).magnitude > _MinSnapshotDistance)
        {
            _ReverseData.Enqueue(new Snapshot(transform.position, transform.rotation, Time.time - _LastTime));
            _LastTime = Time.time;
        }

        return _ReverseData.Size / _ReverseData.Capacity;
    }

    public bool OnReverse()
    {
        if (_ReverseData.Size == 0)
            return false;

        _Selected = false;
        _Hover = false;

        SetOutline();
        StartCoroutine(Reverse());
        return true;
    }

    IEnumerator Reverse()
    {
        if (_ReverseData.Size <= 0)
            yield break;

        _ReverseData.Reverse();

        _Rb.simulated = false;

        Snapshot data = new();
        float delta;

        while (_ReverseData.Size > 0)
        {
            data = _ReverseData.Dequeue();

            while (((Vector2)transform.position - data.pos).magnitude > 0.2f)
            {
                delta = Time.deltaTime / data.time;

                Vector2 pos = Vector2.Lerp(transform.position, data.pos, delta);
                Quaternion rot = Quaternion.Lerp(transform.rotation, data.rot, delta);

                transform.SetPositionAndRotation(pos, rot);

                yield return new WaitForEndOfFrame();
            }

            yield return 0;
        }

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
