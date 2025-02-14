using UnityEngine;

public class Trigger : MonoBehaviour
{
    public static System.Action OnTriggerActivated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnTriggerActivated?.Invoke();
        }
    }
}
