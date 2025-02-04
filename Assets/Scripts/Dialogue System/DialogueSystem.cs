using System;
using UnityEngine;

public enum Dialogue
{
    None,
    Level1Start,
    GunPickup,
    Level1End
}

[Serializable]
public struct DialoguePanel
{
    public Dialogue dialogue;
    public GameObject panel;
    public float delay;
    public float time;
}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] DialoguePanel[] _DialoguePanels;

    DialoguePanel _Current;
    DialoguePanel _Null;

    private void Awake()
    {
        _Null = new DialoguePanel { dialogue = Dialogue.None };
        _Current = _Null;

    }

    private void Start()
    {
        // disable all dialogues
        foreach (DialoguePanel panel in _DialoguePanels)
            if (panel.panel != null)
                panel.panel.SetActive(false);
    }

    private void OnEnable() => GameEvents.UI.OnDialogueTriggered += DialogueTriggered;
    private void OnDisable() => GameEvents.UI.OnDialogueTriggered -= DialogueTriggered;

    private void OnDestroy()
    {
        CancelInvoke(nameof(DisableDialogue));
        CancelInvoke(nameof(EnableDialogue));
    }

    private void DisableDialogue()
    {
        if (_Current.dialogue != Dialogue.None)
        {
            _Current.panel.SetActive(false);
            _Current = _Null;
            if (InputManager.Instance != null)
                InputManager.Instance.SetInput(true);
        }
    }

    private void EnableDialogue()
    {
        // disable inputs
        if (InputManager.Instance != null)
            InputManager.Instance.SetInput(false);
        _Current.panel.SetActive(true);
        Invoke(nameof(DisableDialogue), _Current.time);
    }

    private void DialogueTriggered(Dialogue dialogue)
    {
        if (_Current.dialogue != Dialogue.None)
            return;

        foreach (DialoguePanel panel in _DialoguePanels)
            if (panel.dialogue == dialogue)
                _Current = panel;

        if (_Current.dialogue != Dialogue.None)
            Invoke(nameof(EnableDialogue), _Current.delay);
    }
}
