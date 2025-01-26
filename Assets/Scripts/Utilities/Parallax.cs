using UnityEngine;

public class Parallex : MonoBehaviour
{
    [SerializeField] Vector2 _Speed;
    [SerializeField] bool _FollowX;
    [SerializeField] bool _FollowY;
    [SerializeField] Vector2 _Offset;

    SpriteRenderer _Renderer;
    Transform _MainCamera;

    Vector2 _InitPos;

    private void Awake()
    {
        _MainCamera = Camera.main.transform;
        _Renderer = GetComponent<SpriteRenderer>();

        _InitPos = (Vector2)transform.position + _Offset;
    }

    private void Update()
    {
        if (_MainCamera != null)
        {
            Vector2 camPos = _MainCamera.position;

            Vector2 pos = new(_FollowX ? camPos.x : _InitPos.x, _FollowY ? camPos.y : _InitPos.y);
            transform.position = pos;

            Vector2 offset = camPos - _InitPos;
            _Renderer.material.SetVector("_Offset", offset * _Speed);
        }
    }
}
