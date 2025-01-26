
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _Inputs;
    public static InputManager Instance { get; private set; }   // Singleton Instance

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

        _Inputs = GetComponent<PlayerInput>();
    }

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
        SetInput(_state == GameState.Play || _state == GameState.Start);
    }

    public void SetInput(bool status)
    {
        Cursor.lockState = status ? CursorLockMode.Locked : CursorLockMode.None;
        if (status) _Inputs.currentActionMap.Enable();
        else _Inputs.currentActionMap.Disable();
    }

    //Player Inputs
    public void OnLook(InputValue value) => GameEvents.Input.OnPlayerLook?.Invoke(value.Get<Vector2>());
    public void OnMove(InputValue value) => GameEvents.Input.OnPlayerMove?.Invoke(value.Get<Vector2>());
    public void OnJump(InputValue value) => GameEvents.Input.OnPlayerJump?.Invoke(value.isPressed);
    public void OnSprint(InputValue value) => GameEvents.Input.OnPlayerSprint?.Invoke(value.isPressed);

    //UI Inputs
    public void OnCancel(InputValue value) => GameEvents.Input.OnUICancel?.Invoke();
}
