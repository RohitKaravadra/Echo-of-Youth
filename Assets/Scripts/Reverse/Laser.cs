
using UnityEngine;


public class Laser : MonoBehaviour
{
    [SerializeField] LineRenderer _LaserRenderer;
    [SerializeField] Transform _Particles;

    public bool Enabled { get => gameObject.activeSelf; set { gameObject.SetActive(value); } }
    public void Set(Vector2 start, Vector2 end)
    {
        Vector3[] list = new Vector3[2];
        list[0] = start;
        list[1] = end;

        _Particles.position = end;
        _LaserRenderer.SetPositions(list);
    }
}
