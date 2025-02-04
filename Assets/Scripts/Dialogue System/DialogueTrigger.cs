using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] Dialogue _Dialogue;
    [SerializeField] bool _DisableAfterTrigger;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEvents.UI.OnDialogueTriggered?.Invoke(_Dialogue);
            if (_DisableAfterTrigger)
                gameObject.SetActive(false); // disable to prevent multiple triggers
        }
    }
}
