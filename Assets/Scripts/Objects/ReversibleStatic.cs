
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ReversibleStatic : MonoBehaviour, IInteractable
{
    [SerializeField] int _MaxReverseSteps = 100000; // Practically infinite
    [SerializeField] float _SnapshotThreshold = 0.1f;
    [SerializeField] float _SnapshotTimeThreshold = 0.01f;
    [SerializeField][Range(0, 1)] float _ReverseSpeed = 1;
    [Space(10)]
    [SerializeField] SpriteRenderer _Visuals;

    private Rigidbody2D _Rb;
    private OutlineEffect _Outline;
    Snapshot _InitialSnapshot;

    public Vector2 Position => transform.position;
    private bool triggered = false;
    
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

        // Body is static on creation
        _Rb.bodyType = RigidbodyType2D.Static;

        _Outline.Set(_Visuals);
    }

    private void Start()
    {
        _History = new(_MaxReverseSteps);
        _InitialSnapshot = new Snapshot(transform.position, transform.rotation, 0);
        TakeSnapshot(); // For the love of god please work
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    void Update()
    {
        _LastTime += Time.deltaTime;

        if (_Rb.simulated && (_History.Size == 0 || (_LastTime >= _SnapshotTimeThreshold)))
        {
            if ((_History.Last.pos - _Rb.position).magnitude > _SnapshotThreshold)
            {
                TakeSnapshot();
            }
            _LastTime = 0;
        }
    }

    public bool Compare(Transform other) => transform.Equals(other);

    public void OnHover(bool state, bool selected)
    {
        _Hover = state && !selected;
                SetOutline();
    }

    public void OnSelect(bool state)
    {
        return;
    }

    private void TakeSnapshot()
    {
        _History.Enqueue(new Snapshot(transform.position, transform.rotation, _LastTime));
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

        StartCoroutine(Reverse());
        return true;
    }

    IEnumerator Reverse()
    {
        if (_History.Size <= 0)
            yield break;

        _History.Reverse();

        _Rb.simulated = false;

        Snapshot data;

        while (_History.Size > 0)
        {
            data = _History.Dequeue();
            transform.SetPositionAndRotation(data.pos, data.rot);
            yield return new WaitForSeconds(data.time / _ReverseSpeed);
        }

        yield return 0;

        _Rb.linearVelocity = Vector2.zero;
        _Rb.angularVelocity = 0;

        _Rb.bodyType = RigidbodyType2D.Static;
        _Rb.simulated = true;

        transform.SetPositionAndRotation(_InitialSnapshot.pos, _InitialSnapshot.rot);
    }
    private void SetOutline()
    {
        _Outline.Enabled = _Hover || _Selected;
    }

    public void OnTriggerAction()
    {
        // Activate only once. Ignore if already activated.
        if (triggered)
        {
            Trigger.OnTriggerActivated -= OnTriggerAction;
            return;
        }
        _LastTime = 0; // For correct reversing
        triggered = true;
        _Rb.bodyType = RigidbodyType2D.Dynamic; // Let the box drop

        // Give random velocity
        _Rb.linearVelocity = new Vector2(Random.Range(-2, 2), Random.Range(-2, 2));
        _Rb.angularVelocity = Random.Range(-50, 50);
    }

    private void OnEnable()
    {
        // Subscribe to trigger
        Trigger.OnTriggerActivated += OnTriggerAction;
    }

    private void OnDisable()
    {
        if (!triggered)
        {
            // Unsubscribe from trigger
            Trigger.OnTriggerActivated -= OnTriggerAction;
        }

    }
}
