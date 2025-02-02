
using System;
using UnityEngine;

public enum AudioFile
{
    Reverse,
    Jump
}

[Serializable]
public struct AudioFileData
{
    public AudioFile type;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource _MusicSource;
    [SerializeField] AudioSource _PlayerSource;
    [SerializeField] AudioSource _LaserSource;
    [Space(10)]
    [SerializeField] AudioFileData[] _AudioFiles;

    public static AudioManager Instance { get; private set; }  // Singleton Instance
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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public static float DbToPr(float db)
    {
        if (db <= -80f)  // Clamp values near -80 dB
            return 0f;
        return Mathf.Pow(10, db / 20) * 100f;
    }

    public static float PrToDb(float pr)
    {
        if (pr <= 0.01f)  // Prevent log(0) error
            return -80f;
        return 20f * Mathf.Log10(pr / 100f);
    }

    public void PlayMusic() => _MusicSource.Play();
    public void StopMusic() => _MusicSource.Stop();
    public void PlayLaser() => _LaserSource.Play();
    public void StopLaser() => _LaserSource.Stop();
    public void PlaySound(AudioFile _type)
    {
        foreach (var af in _AudioFiles)
            if (af.type == _type)
                _PlayerSource.PlayOneShot(af.clip);
    }
}
