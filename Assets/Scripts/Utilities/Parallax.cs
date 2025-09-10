using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Vector2 _Speed;
    [SerializeField][Range(0.01f, 5f)] float _MaxThreshold;

    Vector2 _LastCamPos;
    Transform _Camera;

    private void Start()
    {
        _Camera = Camera.main.transform;
        _LastCamPos = _Camera.position;
    }

    private void Update()
    {
        Vector2 newCamPos = _Camera.position;
        Vector2 offset = newCamPos - _LastCamPos;

        _LastCamPos = newCamPos;

        if (offset.magnitude > _MaxThreshold)
            return;

        transform.position = (Vector2)transform.position - offset * _Speed;
    }
}
