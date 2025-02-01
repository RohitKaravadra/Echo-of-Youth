using UnityEngine;

public interface IReversible
{
    public void OnHover(bool state);
    public void OnSelect(bool state);
    public float OnMove(Vector2 position);
    public bool OnReverse();
    public bool Compare(Transform other);
}
