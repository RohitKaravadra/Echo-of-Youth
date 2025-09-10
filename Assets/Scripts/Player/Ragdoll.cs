using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Ragdoll : MonoBehaviour
{
    [SerializeField] Rigidbody2D[] _Rigidbodies;
    [SerializeField] Collider2D[] _Colliders;
    [SerializeField] float _Force;

    Animator _Animator;
    private void Awake() => _Animator = GetComponent<Animator>();
    private void Start() => Disable();

    public void Enable()
    {
        _Animator.enabled = false;

        foreach (var c in _Colliders)
            c.enabled = true;

        foreach (var b in _Rigidbodies)
        {
            b.simulated = true;
            b.AddForce(new Vector2(Random.Range(-_Force, _Force), Random.Range(-_Force, _Force)), ForceMode2D.Impulse);
        }

    }

    public void Disable()
    {
        foreach (var b in _Rigidbodies)
        {
            b.simulated = false;
            b.transform.localRotation = Quaternion.identity;
        }

        foreach (var c in _Colliders)
            c.enabled = false;

        _Animator.enabled = true;
    }
}
