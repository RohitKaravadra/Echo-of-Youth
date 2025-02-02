using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class EventTrigger : MonoBehaviour
{
    [SerializeField] TriggerEvents _Event;
    bool _triggered = false;

    public void CallTrigger(TriggerEvents _event)
    {
        if (_triggered)
            return;

        _triggered = true;
        switch (_event)
        {
            case TriggerEvents.GameOver:
                GameEvents.Game.OnLevelOver?.Invoke();
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            CallTrigger(_Event);
    }
}
