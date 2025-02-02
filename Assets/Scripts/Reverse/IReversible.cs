using UnityEngine;

public interface IReversible
{
    public Vector2 Position { get; }
    public void OnHover(bool state, bool selected = false);
    public void OnSelect(bool state);
    public float OnMove(Vector2 position);
    public bool OnReverse();
    public bool Compare(Transform other);
}
