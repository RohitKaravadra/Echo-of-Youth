using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Scenes _NextScene;
    [SerializeField] float _LoadAfterTime = 2.0f;
    bool _Pause = false;
    public static GameManager Instance { get; private set; }    // Singleton Instances
    public static bool HasInstance => Instance != null;

    bool _LevelOver;
    private void Awake()
    {
        // Singleton implementation
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("Instance of " + this.name + " already exists \n deleting this instance");
            Destroy(this);
        }
    }

    private void Start()
    {
        _LevelOver = false;
        Time.timeScale = 1.0f;
        GameEvents.Input.OnSetInputState?.Invoke(!_Pause);
    }

    private void OnEnable()
    {
        GameEvents.Input.OnUICancel += OnUICancel;
        GameEvents.Game.OnLevelOver += LevelOver;
    }
    private void OnDisable()
    {
        GameEvents.Input.OnUICancel -= OnUICancel;
        GameEvents.Game.OnLevelOver -= LevelOver;
    }

    private void OnDestroy()
    {
        CancelInvoke(nameof(LoadNextScene));
        if (Instance == this)
            Instance = null;
    }

    private void OnUICancel()
    {
        if (_LevelOver)
            return;

        _Pause = !_Pause;
        Time.timeScale = _Pause ? 0.0f : 1.0f;
        GameEvents.Input.OnSetInputState?.Invoke(!_Pause);
        GameEvents.Game.OnGamePause?.Invoke(_Pause);
    }

    private void LoadNextScene() => SceneManager.LoadScene((int)_NextScene);

    private void LevelOver()
    {
        _LevelOver = true;
        GameEvents.Input.OnSetInputState?.Invoke(false);
        Invoke(nameof(LoadNextScene), _LoadAfterTime);
    }
}
