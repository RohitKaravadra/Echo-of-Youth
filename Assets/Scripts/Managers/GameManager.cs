using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Scenes _NextScene;
    bool _Pause = false;
    public static GameManager Instance { get; private set; }    // Singleton Instances
    public static bool HasInstance => Instance != null;
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
        _Pause = !_Pause;
        GameEvents.Input.OnSetInputState?.Invoke(!_Pause);
        GameEvents.Game.OnGamePause?.Invoke(_Pause);
    }

    private void LoadNextScene() => SceneManager.LoadScene((int)_NextScene);

    private void LevelOver()
    {
        GameEvents.Input.OnSetInputState?.Invoke(false);
        Invoke(nameof(LoadNextScene), 2f);
    }
}
