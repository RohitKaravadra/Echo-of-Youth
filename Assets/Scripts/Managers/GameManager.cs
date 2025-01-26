using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameState _GameState;
    public static GameManager Instance { get; private set; }    // Singleton Instances

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
        // start game 
        UpdateGameState(GameState.Play);
    }

    private void OnEnable()
    {
        GameEvents.Input.OnUICancel += OnUICancel;
    }
    private void OnDisable()
    {
        GameEvents.Input.OnUICancel -= OnUICancel;
    }

    private void UpdateGameState(GameState newState)
    {
        _GameState = newState == GameState.Start ? GameState.Play : newState;

        Time.timeScale = _GameState == GameState.Pause ? 0 : 1;
        GameEvents.Game.OnGameStateChanged(newState);
    }

    private void OnUICancel()
    {
        if (_GameState == GameState.Play || _GameState == GameState.Pause)
            UpdateGameState(_GameState == GameState.Play ? GameState.Pause : GameState.Play);
    }
}
