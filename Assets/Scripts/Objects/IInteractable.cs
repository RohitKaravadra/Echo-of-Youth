using UnityEngine;

public interface IInteractable
{
    public Vector2 Position { get; }
    public void OnHover(bool state, bool selected = false);
    public void OnSelect(bool state);
    public void OnMove(Vector2 position);
    public bool OnReverse();
    public bool Compare(Transform other);
}
