using System;
using System.Collections.Generic;
using System.Linq;
using Managers.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private List<SoundData> sounds = new();
        [SerializeField] private List<ThemeData> themes = new();

        private AudioSource mainAudio = null;

        #region Singleton

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        public void PlaySound(ESoundType soundType)
        {
            var clip = sounds.FirstOrDefault(s => s.SoundType == soundType)?.AudioClip;
            if (clip == null) return;

            var audioSource = new GameObject($"Audio: {soundType}", typeof(AudioSource));
            var audioSourceScript = audioSource.GetComponent<AudioSource>();
            audioSourceScript.volume = PlayerPrefs.GetFloat(StaticOptions.PLAYER_PREF_SFX_VOLUME, 0.1f);
            audioSourceScript.loop = false;
            audioSourceScript.pitch -= Random.Range(-0.1f, 0.1f);
            audioSourceScript.PlayOneShot(clip);
            
            Destroy(audioSource, 0.5f);
        }

        public void SetTheme(EThemeType themeType)
        {
            var audioSource = mainAudio;
            if (audioSource == null)
            {
                var audioSourceObj = new GameObject($"Audio: {themeType}", typeof(AudioSource));
                audioSource = audioSourceObj.GetComponent<AudioSource>();
                mainAudio = audioSource;
            }
            
            var clip = themes.FirstOrDefault(s => s.ThemeType == themeType)?.AudioClip;
            if (clip == null) return;

            audioSource.clip = clip;
            audioSource.volume = PlayerPrefs.GetFloat(StaticOptions.PLAYER_PREF_VOLUME, 0.3f);
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    [System.Serializable]
    public class SoundData
    {
        public AudioClip AudioClip;
        public ESoundType SoundType;
    }
    
    [System.Serializable]
    public class ThemeData
    {
        public AudioClip AudioClip;
        public EThemeType ThemeType;
    }
}