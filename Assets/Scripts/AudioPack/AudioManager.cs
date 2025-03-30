using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Enums;
using Managers.Other;
using PoolPack;
using UnityEngine;
using UnityEngine.Pool;

namespace AudioPack
{
    public class AudioManager : PoolManager
    {
        [SerializeField] private List<SoundData> sounds = new();
        [SerializeField] private List<ThemeData> themes = new();

        private AudioSource mainAudio = null;

        private ObjectPool<SFXPoolObject> _sfxPool;

        public IEnumerable<SoundData> AllSounds => sounds;
        
        #region Singleton

        private static AudioManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;

            GameManager.OnStartRun += SetThemeForStage;
            GameManager.OnInit += Init;
        }

        #endregion

        private void Init()
        {
            var sfxPrefab = GameManager.Instance.GetPrefab<SFXPoolObject>(PrefabNames.SFX);
            _sfxPool = PoolHelper.CreatePool(this, sfxPrefab, false);
            
            PrepareQueue();
        }

        private void OnDisable()
        {
            GameManager.OnStartRun -= SetThemeForStage;
            GameManager.OnInit -= Init;
        }

        private void SetThemeForStage()
        {
            // TODO: ustaw odpowiedni theme
            SetTheme(EThemeType.Mines);
        }

        public static void PlaySound(ESoundType soundType)
        {
            Instance.GetPoolObject<SFXPoolObject>().Play(soundType);
        }

        public static void SetTheme(EThemeType themeType)
        {
            var audioSource = Instance.mainAudio;
            if (audioSource == null)
            {
                var audioSourceObj = new GameObject($"Audio: {themeType}", typeof(AudioSource));
                audioSource = audioSourceObj.GetComponent<AudioSource>();
                Instance.mainAudio = audioSource;
            }
            
            var clip = Instance.themes.FirstOrDefault(s => s.ThemeType == themeType)?.AudioClip;
            if (clip == null) return;

            audioSource.clip = clip;
            audioSource.volume = PlayerPrefs.GetFloat(StaticOptions.PLAYER_PREF_VOLUME, 0.3f);
            audioSource.loop = true;
            audioSource.Play();
        }

        protected override T GetPoolObject<T>()
        {
            return _sfxPool.Get() as T;
        }

        public override void ReleasePoolObject(PoolObject poolObject)
        {
            _sfxPool.Release(poolObject as SFXPoolObject);
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