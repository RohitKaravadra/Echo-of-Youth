using Cinemachine;
using System.Collections;
using UnityEngine;

[System.Serializable]
public struct ShakeData
{
    public float magnitude;
    public float frequency;
    public float time;
}

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _FollowCamera;
    [Space(10)]
    [SerializeField] bool _DisableShake;
    [SerializeField] AnimationCurve _MagnitudeCurve;
    [SerializeField] AnimationCurve _FrequencyCurve;

    public Transform FollowTarget { get => _FollowCamera.Follow; set { _FollowCamera.Follow = value; } }
    public static CameraManager Instance { get; private set; }  // Singleton Instance

    private ShakeData _ShakeData;
    private float _CurTime;
    private bool _IsShaking;

    private CinemachineBasicMultiChannelPerlin _Noise;

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

        _Noise = _FollowCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    /// <summary> Apply camera shake using noise </summary>\
    /// <param name="magnitude"> Magnitude of Noise </param>
    /// <param name="frequency"> Frequency of Noise (per sec)</param>
    /// <param name="time"> Timeperiod of Noise </param>
    public void ApplyShake(ShakeData data)
    {
        if (_Noise == null || _DisableShake) return;

        _ShakeData = data;
        _CurTime = _ShakeData.time;

        if (!_IsShaking)
            StartCoroutine(nameof(StartShake));
    }
    private IEnumerator StartShake()
    {
        _IsShaking = true;
        while (_CurTime > 0)
        {
            float factor = 1.0f - (_CurTime / _ShakeData.time);
            _Noise.m_AmplitudeGain = _ShakeData.magnitude * _MagnitudeCurve.Evaluate(factor);
            _Noise.m_FrequencyGain = _ShakeData.frequency * _FrequencyCurve.Evaluate(factor);

            _CurTime -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        _CurTime = 0;
        _IsShaking = false;

        _Noise.m_AmplitudeGain = 0;
        _Noise.m_FrequencyGain = 0;
    }

}
