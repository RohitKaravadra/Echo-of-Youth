
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
        GameEvents.Input.OnSetInputState += SetInput;
    }

    private void OnDisable()
    {
        GameEvents.Input.OnSetInputState -= SetInput;
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
    public void OnSelect(InputValue value) => GameEvents.Input.OnObjectSelect?.Invoke(value.isPressed);
    public void OnReverse(InputValue value) => GameEvents.Input.OnObjectReverse?.Invoke(value.isPressed);

    //UI Inputs
    public void OnCancel(InputValue value) => GameEvents.Input.OnUICancel?.Invoke();
}
