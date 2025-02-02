using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _PausePanel;

    private void OnEnable()
    {
        GameEvents.Game.OnGamePause += PauseGame;
    }

    private void OnDisable()
    {
        GameEvents.Game.OnGamePause -= PauseGame;
    }

    private void PauseGame(bool pause) => _PausePanel.SetActive(pause);

    public void OnResumeButton()
    {
        GameEvents.Input.OnUICancel.Invoke();
    }

    public void OnExitButton()
    {
        SceneManager.LoadScene((int)Scenes.MainMenu);
    }
}
