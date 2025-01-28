using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _PausePanel;

    private void OnEnable()
    {
        GameEvents.Game.OnGameStateChanged += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameEvents.Game.OnGameStateChanged -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState _state)
    {
        _PausePanel.SetActive(_state == GameState.Pause);
    }

    public void OnResumeButton()
    {
        GameEvents.Input.OnUICancel.Invoke();
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene((int)Scenes.MainMenu);
    }
}
