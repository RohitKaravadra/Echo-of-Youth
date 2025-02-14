
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ReversibleStatic : MonoBehaviour, IInteractable
{
    [SerializeField] int _MaxReverseSteps = 100;
    [SerializeField] float _SnapshotThreshold = 0.1f;
    [SerializeField] float _TrailThreshold = 0.5f;
    [SerializeField][Range(0, 1)] float _ReverseSpeed = 1;
    [Space(10)]
    [SerializeField] SpriteRenderer _Visuals;
    [SerializeField] LineRenderer _TrailRenderer;

    private Rigidbody2D _Rb;
    private OutlineEffect _Outline;

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
    bool _Triggered = false;

    private void Awake()
    {
        _Outline = GetComponent<OutlineEffect>();
        _Rb = GetComponent<Rigidbody2D>();

        // Body is static on creation
        _Rb.bodyType = RigidbodyType2D.Static;

        _Outline.Set(_Visuals);
    }

    private void Start() => _History = new(_MaxReverseSteps);

    private void OnEnable() => Trigger.OnTriggerActivated += OnTriggerAction;
    private void OnDisable() => Trigger.OnTriggerActivated -= OnTriggerAction;

    private void OnDestroy() => StopAllCoroutines();

    void Update()
    {
        _TrailRenderer.enabled = _Outline.Enabled;

        if (_Triggered)
            if (_History.Size == 0 || (_History.Last.pos - _Rb.position).magnitude > _SnapshotThreshold)
                TakeSnapshot();
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
    private void TakeSnapshot()
    {
        _History.Enqueue(new Snapshot(transform.position, transform.rotation, Time.time - _LastTime));
        _LastTime = Time.time;
        SetTrail();
    }

    public bool Compare(Transform other) => transform.Equals(other);

    public void OnHover(bool state)
    {
        _Hover = state;
        SetOutline();
    }

    public void OnSelect(bool state)
    {
        return;
    }

    public void OnMove(Vector2 position)
    {
        return;
    }

    public bool OnReverse()
    {
        if (_History.Size == 0)
            return false;

        _Selected = false;
        _Hover = false;
        _Triggered = false;

        StartCoroutine(Reverse());
        return true;
    }

    IEnumerator Reverse()
    {
        if (_History.Size <= 0)
            yield break;

        _History.Reverse();

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

        transform.SetPositionAndRotation(data.pos, data.rot);
        _TrailRenderer.enabled = false;

        _Rb.bodyType = RigidbodyType2D.Static;
        _Rb.simulated = true;
    }

    private void SetOutline()
    {
        _Outline.Enabled = _Hover || _Selected;
        if (_Outline.Enabled)
            SetTrail();
    }

    public void OnTriggerAction()
    {
        Trigger.OnTriggerActivated -= OnTriggerAction;
        
        _LastTime = Time.time;
        TakeSnapshot();
        _Triggered = true;
        _Rb.bodyType = RigidbodyType2D.Dynamic; // Let the box drop

        // Give random velocity
        _Rb.linearVelocity = new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
        _Rb.angularVelocity = Random.Range(-50, 50);
    }
}
