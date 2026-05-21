using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    [Header("±≥æ∞“Ù¿÷")]
    public AudioSource bgmSource;      // Õœ AudioSource
    public float bgmVolume = 0.7f;
    public float fadeDuration = 1f;

    //[Header("“Ù–ßÕ®µ¿")]
    //public AudioSource sfxSource;      // Õœ¡Ì“ª∏ˆ AudioSource
    //public float sfxVolume = 0.8f;

    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    _instance = obj.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
}

    private void Update()
    {
        bgmSource.volume = bgmVolume;
    }

    /* ---------- ±≥æ∞“Ù¿÷ ---------- */
    public void PlayBGM(AudioClip clip, bool fade = true)
    {
        if (clip == null) return;
        StartCoroutine(FadeSwitch(clip, fade));
    }

    private IEnumerator FadeSwitch(AudioClip newClip, bool fade)
    {
        if (fade && bgmSource.isPlaying)
        {
            // µ≠≥ˆ
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(bgmVolume, 0, t / fadeDuration);
                yield return null;
            }
        }

        bgmSource.clip = newClip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();

        if (fade)
        {
            // µ≠»Î
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0, bgmVolume, t / fadeDuration);
                yield return null;
            }
        }
    }

    public void SetBGMVolume(float vol)
    {
        bgmVolume = Mathf.Clamp01(vol);
        bgmSource.volume = bgmVolume;
    }

    public void ToggleBGMMute()
    {
        bgmSource.mute = !bgmSource.mute;
    }

    ///* ---------- “Ù–ß ---------- */
    //public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    //{
    //    if (clip == null) return;
    //    sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    //}

    //public void SetSFXVolume(float vol)
    //{
    //    sfxVolume = Mathf.Clamp01(vol);
    //    sfxSource.volume = sfxVolume;
    //}

    //public void ToggleSFXMute()
    //{
    //    sfxSource.mute = !sfxSource.mute;
    //}
}