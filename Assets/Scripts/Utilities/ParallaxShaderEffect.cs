using UnityEngine;

public class ParallexShaderEffect : MonoBehaviour
{
    [SerializeField] Vector2 _ScrollSpeed;
    [SerializeField] bool _FollowCameraX;
    [SerializeField] bool _FollowCameraY;

    SpriteRenderer _Renderer;

    Vector2 _LastCamPos;
    Transform _Camera;
    Vector2 _Offset;

    private void Awake()
    {
        _Camera = Camera.main.transform;
        _Renderer = GetComponent<SpriteRenderer>();

        _LastCamPos = _Camera.position;
    }

    private void Update()
    {
        if (_Camera != null)
        {
            Vector2 newCamPos = _Camera.position;
            Vector2 offset = newCamPos - _LastCamPos;

            _LastCamPos = newCamPos;

            Vector2 newPos = transform.position;

            if (_FollowCameraX)
                newPos.x = _Camera.position.x;
            if (_FollowCameraY)
                newPos.y = _Camera.position.y;

            transform.position = newPos;

            if (offset.magnitude < 0.5f)
            {
                _Offset += offset * _ScrollSpeed;
                _Renderer.material.SetVector("_Offset", _Offset);
            }
        }
    }
}
