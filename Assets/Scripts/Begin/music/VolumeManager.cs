using UnityEngine;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [SerializeField] private AudioSource audioMixer;

    [SerializeField] private string masterVolumeParam = "MasterVolume";
    [SerializeField] private string musicVolumeParam = "MusicVolume";
    [SerializeField] private string sfxVolumeParam = "SFXVolume";

    private float masterVolume = 0.7f;
    private float musicVolume = 0.7f;
    private float sfxVolume = 0.8f;

    private float masterBeforeMute = 0.7f;
    private float musicBeforeMute = 0.7f;
    private float sfxBeforeMute = 0.8f;

    private bool isMasterMuted = false;
    private bool isMusicMuted = false;
    private bool isSFXMuted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAllSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadAllSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.7f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);

        isMasterMuted = PlayerPrefs.GetInt("MasterMuted", 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        isSFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;

        masterBeforeMute = PlayerPrefs.GetFloat("MasterBeforeMute", masterVolume);
        musicBeforeMute = PlayerPrefs.GetFloat("MusicBeforeMute", musicVolume);
        sfxBeforeMute = PlayerPrefs.GetFloat("SFXBeforeMute", sfxVolume);

        ApplyAllVolumes();
    }

    void SaveAllSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        PlayerPrefs.SetInt("MasterMuted", isMasterMuted ? 1 : 0);
        PlayerPrefs.SetInt("MusicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFXMuted", isSFXMuted ? 1 : 0);

        PlayerPrefs.SetFloat("MasterBeforeMute", masterBeforeMute);
        PlayerPrefs.SetFloat("MusicBeforeMute", musicBeforeMute);
        PlayerPrefs.SetFloat("SFXBeforeMute", sfxBeforeMute);

        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (isMasterMuted && volume > 0.01f)
        {
            isMasterMuted = false;
        }

        if (!isMasterMuted)
        {
            masterVolume = volume;
            masterBeforeMute = volume;

            if (volume <= 0.01f)
            {
                isMasterMuted = true;
            }
        }
        else
        {
            masterVolume = 0f;
        }

        ApplyMasterVolume();
        SaveAllSettings();
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (isMusicMuted && volume > 0.01f)
        {
            isMusicMuted = false;
        }

        if (!isMusicMuted)
        {
            musicVolume = volume;
            musicBeforeMute = volume;

            if (volume <= 0.01f)
            {
                isMusicMuted = true;
            }
        }
        else
        {
            musicVolume = 0f;
        }

        ApplyMusicVolume();
        SaveAllSettings();
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (isSFXMuted && volume > 0.01f)
        {
            isSFXMuted = false;
        }

        if (!isSFXMuted)
        {
            sfxVolume = volume;
            sfxBeforeMute = volume;

            if (volume <= 0.01f)
            {
                isSFXMuted = true;
            }
        }
        else
        {
            sfxVolume = 0f;
        }

        ApplySFXVolume();
        SaveAllSettings();
    }

    public void ToggleMasterMute()
    {
        isMasterMuted = !isMasterMuted;

        if (isMasterMuted)
        {
            masterBeforeMute = masterVolume > 0.01f ? masterVolume : masterBeforeMute;
            masterVolume = 0f;
        }
        else
        {
            masterVolume = masterBeforeMute;
        }

        ApplyMasterVolume();
        SaveAllSettings();
    }

    public void ToggleMusicMute()
    {
        isMusicMuted = !isMusicMuted;

        if (isMusicMuted)
        {
            musicBeforeMute = musicVolume > 0.01f ? musicVolume : musicBeforeMute;
            musicVolume = 0f;
        }
        else
        {
            musicVolume = musicBeforeMute==0?10:musicBeforeMute;
        }

        ApplyMusicVolume();
        SaveAllSettings();
    }

    public void ToggleSFXMute()
    {
        isSFXMuted = !isSFXMuted;

        if (isSFXMuted)
        {
            sfxBeforeMute = sfxVolume > 0.01f ? sfxVolume : sfxBeforeMute;
            sfxVolume = 0f;
        }
        else
        {
            sfxVolume = sfxBeforeMute;
        }

        ApplySFXVolume();
        SaveAllSettings();
    }

    void ApplyMasterVolume()
    {
        float volumeToApply = isMasterMuted ? 0f : masterVolume;

        if (audioMixer != null)
        {
            float dbVolume = volumeToApply > 0.001f ? 20f * Mathf.Log10(volumeToApply) : -80f;
            //audioMixer.SetFloat(masterVolumeParam, dbVolume);
        }
        else
        {
            AudioListener.volume = volumeToApply;
        }
    }

    void ApplyMusicVolume()
    {
        float volumeToApply = isMusicMuted ? 0f : musicVolume;

        AudioManager.Instance.bgmVolume = volumeToApply;
    }

    void ApplySFXVolume()
    {
        float volumeToApply = isSFXMuted ? 0f : sfxVolume;

        if (audioMixer != null)
        {
            float dbVolume = volumeToApply > 0.001f ? 20f * Mathf.Log10(volumeToApply) : -80f;
            //audioMixer.SetFloat(sfxVolumeParam, dbVolume);
        }
    }

    void ApplyAllVolumes()
    {
        ApplyMasterVolume();
        ApplyMusicVolume();
        ApplySFXVolume();
    }

    public float GetMasterVolume() => isMasterMuted ? 0f : masterVolume;
    public float GetMusicVolume() => isMusicMuted ? 0f : musicVolume;
    public float GetSFXVolume() => isSFXMuted ? 0f : sfxVolume;

    public bool IsMasterMuted() => isMasterMuted;
    public bool IsMusicMuted() => isMusicMuted;
    public bool IsSFXMuted() => isSFXMuted;

    public void ResetToDefaults()
    {
        masterVolume = 0.7f;
        musicVolume = 0.7f;
        sfxVolume = 0.8f;
        masterBeforeMute = 0.7f;
        musicBeforeMute = 0.7f;
        sfxBeforeMute = 0.8f;

        isMasterMuted = false;
        isMusicMuted = false;
        isSFXMuted = false;

        ApplyAllVolumes();
        SaveAllSettings();
    }
}

