using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CrosshairCollision : MonoBehaviour
{
    public static Action<Transform, bool> OnObjectHover;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor"))
            OnObjectHover?.Invoke(transform, true);

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor"))
            OnObjectHover?.Invoke(transform, false);
    }
}
