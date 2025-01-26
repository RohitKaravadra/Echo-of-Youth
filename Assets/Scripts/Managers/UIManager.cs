using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _PausePanel;
    [Space(5)]
    [SerializeField] Button _AirControlButton;                   // **for testing only
    [SerializeField] TextMeshProUGUI _AirControlButtonText;      // **for testing only

    private void OnEnable()
    {
        _AirControlButton.onClick.AddListener(OnAirControlButton);
        GameEvents.Game.OnGameStateChanged += OnGameStateChange;
    }

    private void OnDisable()
    {
        _AirControlButton.onClick.RemoveListener(OnAirControlButton);
        GameEvents.Game.OnGameStateChanged -= OnGameStateChange;
    }

    private void OnGameStateChange(GameState _state)
    {
        _PausePanel.SetActive(_state == GameState.Pause);
    }

    // **for testing only
    private void OnAirControlButton()
    {
        Settings.s_AirControlEnabled = !Settings.s_AirControlEnabled;
        _AirControlButtonText.text = Settings.s_AirControlEnabled ? "Air Control Enabled" : "Air Control Disabled";
        GameEvents.UI.OnAirControlChanged?.Invoke();
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
