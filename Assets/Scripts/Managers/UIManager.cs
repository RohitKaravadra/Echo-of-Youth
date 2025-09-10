using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject _PausePanel;
    [Space(10)]
    [SerializeField] Slider _MasterSlider;
    [SerializeField] Slider _MusicSlider;
    [SerializeField] Slider _SFXSlider;
    [SerializeField] AudioMixer _Mixer;

    private void Start()
    {
        SetVolume();
        SetSliders();
    }

    private void OnEnable()
    {
        GameEvents.Game.OnGamePause += PauseGame;
    }

    private void OnDisable()
    {
        GameEvents.Game.OnGamePause -= PauseGame;
    }
    private void SetSliders()
    {
        _MasterSlider.onValueChanged.AddListener((val) =>
        {
            _Mixer.SetFloat("Master", AudioManager.PrToDb(val));
            PlayerPrefs.SetFloat("Master", val);
        });
        _MusicSlider.onValueChanged.AddListener((val) =>
        {
            _Mixer.SetFloat("Music", AudioManager.PrToDb(val));
            PlayerPrefs.SetFloat("Music", val);
        });
        _SFXSlider.onValueChanged.AddListener((val) =>
        {
            _Mixer.SetFloat("SFX", AudioManager.PrToDb(val));
            PlayerPrefs.SetFloat("SFX", val);
        });
    }

    private void SetVolume()
    {
        float val = PlayerPrefs.GetFloat("Master", 100);
        _Mixer.SetFloat("Master", AudioManager.PrToDb(val));
        _MasterSlider.value = val;

        val = PlayerPrefs.GetFloat("Music", 100);
        _Mixer.SetFloat("Music", AudioManager.PrToDb(val));
        _MusicSlider.value = val;

        val = PlayerPrefs.GetFloat("SFX", 100);
        _Mixer.SetFloat("SFX", AudioManager.PrToDb(val));
        _SFXSlider.value = val;

    }

    private void PauseGame(bool pause) => _PausePanel.SetActive(pause);

    public void OnResumeButton() => GameEvents.Input.OnUICancel.Invoke();
    public void OnRestartButton() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void OnExitButton()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene((int)Scenes.MainMenu);
    }

    public void OnMastreVolumeSlider(float val)
    {
        print("Master");
    }

    public void OnMusicVolumeSlider(float val)
    {
        print("Music");
    }

    public void OnSFXVolumeSlider(float val)
    {
        print("SFX");
    }
}
