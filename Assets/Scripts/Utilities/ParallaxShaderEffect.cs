using UnityEngine;

public class ParallexShaderEffect : MonoBehaviour
{
    [SerializeField] Vector2 _ScrollSpeed;
    [SerializeField] Vector2 _Offset;
    [SerializeField] float _Scale = 1f;
    [SerializeField] bool _WrapX = true;
    [SerializeField] bool _WrapY = true;

    SpriteRenderer _Renderer;

    Vector2 _LastCamPos;
    Transform _Camera;
    bool _HasRenderer = false;

    private void OnValidate()
    {
        if (_Renderer == null)
            _Renderer = GetComponent<SpriteRenderer>();
        if (_Renderer != null)
        {
            _Renderer.sharedMaterial.SetFloat("_Scale", _Scale);
            _Renderer.sharedMaterial.SetVector("_Offset", _Offset);
            _Renderer.sharedMaterial.SetInt("_Wrap_X", _WrapX ? 1 : 0);
            _Renderer.sharedMaterial.SetInt("_Wrap_Y", _WrapY ? 1 : 0);
        }
    }

    private void Awake()
    {
        _Camera = Camera.main.transform;

        if (_Renderer == null)
            _HasRenderer = TryGetComponent(out _Renderer);
        else
            _HasRenderer = true;

        _LastCamPos = _Camera.position;

        if (_HasRenderer)
        {
            _Renderer.material.SetFloat("_Scale", _Scale);
            _Renderer.material.SetVector("_Offset", _Offset);
            _Renderer.material.SetInt("_Wrap_X", _WrapX ? 1 : 0);
            _Renderer.material.SetInt("_Wrap_Y", _WrapY ? 1 : 0);
        }
    }

    private void Update()
    {
        if (_Camera != null)
        {
            Vector2 newCamPos = _Camera.position;
            Vector2 offset = newCamPos - _LastCamPos;

            if (offset.magnitude > 0.01f)
            {
                _LastCamPos = newCamPos;
                _Offset += offset * _ScrollSpeed;

                if (_HasRenderer)
                {
                    _Renderer.material.SetFloat("_Scale", _Scale);
                    _Renderer.material.SetVector("_Offset", _Offset);
                    _Renderer.material.SetInt("_Wrap_X", _WrapX ? 1 : 0);
                    _Renderer.material.SetInt("_Wrap_Y", _WrapY ? 1 : 0);
                }
            }
        }
    }
}
