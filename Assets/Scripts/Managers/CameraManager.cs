using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _FollowCamera;

    public Transform FollowTarget { get => _FollowCamera.Follow; set { _FollowCamera.Follow = value; } }
    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("Instance of " + this.name + " already exists \n deleting this instance");
            Destroy(this);
        }
    }
}
