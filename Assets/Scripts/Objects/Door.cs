
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] bool _Opened = false;
    [SerializeField] Collider2D _Collider;
    [SerializeField] SpriteRenderer _Renderer;

    private void UpdateState()
    {
        _Renderer.enabled = !_Opened;
        _Collider.enabled = !_Opened;
    }

    public void OnButtonPressed(bool pressed)
    {
        _Opened = pressed;
        UpdateState();
    }
}
