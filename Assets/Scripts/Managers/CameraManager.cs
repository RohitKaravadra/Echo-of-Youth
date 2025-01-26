using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _FollowCamera;

    public Transform FollowTarget { get => _FollowCamera.Follow; set { _FollowCamera.Follow = value; } }
    public static CameraManager Instance { get; private set; }  // Singleton Instance

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
}
