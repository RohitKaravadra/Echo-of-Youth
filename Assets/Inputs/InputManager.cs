
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _Inputs;

    public static Action<Vector2> OnPlayerMove;
    public static Action<Vector2> OnPlayerLook;
    public static Action<bool> OnPlayerJump;
    public static Action<bool> OnPlayerSprint;
    public static Action OnCancelButton;

    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _Inputs = GetComponent<PlayerInput>();
        }
        else
        {
            Debug.LogWarning("Instance of " + this.name + " already exists \n deleting this instance");
            Destroy(this);
        }
    }

    public void OnGameOver()
    {
        SetInput(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetInput(bool status)
    {
        if (status) _Inputs.currentActionMap.Enable();
        else _Inputs.currentActionMap.Disable();
    }

    //Player Inputs
    public void OnLook(InputValue value) => OnPlayerLook?.Invoke(value.Get<Vector2>());
    public void OnMove(InputValue value) => OnPlayerMove?.Invoke(value.Get<Vector2>());
    public void OnJump(InputValue value) => OnPlayerJump?.Invoke(value.isPressed);
    public void OnSprint(InputValue value) => OnPlayerSprint?.Invoke(value.isPressed);

    //UI Inputs
    public void OnCancel(InputValue value) => OnCancelButton?.Invoke();
}
