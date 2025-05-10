using System.Collections.Generic;
using System.Linq;
using Managers.Enums;
using Managers.Other;
using PoolPack;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioPack
{
    public class SFXPoolObject : SimplePoolObject
    {
        private Dictionary<ESoundType, SoundData> _sounds = new();
        private AudioSource _audioSource;

        private AudioManager _audioManager;

        private float _clipLength = 999;

        private float _timer = 0;
        
        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);

            _audioSource = GetComponent<AudioSource>();
            _audioManager = (AudioManager)poolManager;
            _sounds = _audioManager.AllSounds.ToDictionary(s => s.SoundType, s => s);
        }

        public override void OnRelease()
        {
            base.OnRelease();

            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.pitch = 1;
        }

        public void Play(ESoundType soundType)
        {
            if (!_sounds.TryGetValue(soundType, out var clipData)) return;

            _audioSource.volume = PlayerPrefs.GetFloat(StaticOptions.PLAYER_PREF_SFX_VOLUME, 0.1f);
            _audioSource.loop = false;
            _audioSource.pitch -= Random.Range(-0.1f, 0.1f);
            _audioSource.PlayOneShot(clipData.AudioClip);
            _clipLength = clipData.AudioClip.length;

            _timer = 0;
            
            OnGet(null);
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < _clipLength) return;
            
            _audioManager.ReleasePoolObject(this);
        }
    }
}