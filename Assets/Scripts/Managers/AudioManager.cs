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

        #region Singleton

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        public void PlaySound(ESoundType soundType, float volumeScale = 1)
        {
            var clip = sounds.FirstOrDefault(s => s.SoundType == soundType)?.AudioClip;
            if (clip == null) return;

            var audioSource = new GameObject($"Audio: {soundType}", typeof(AudioSource));
            var audioSourceScript = audioSource.GetComponent<AudioSource>();
            audioSourceScript.volume = 0.3f * volumeScale;
            audioSourceScript.loop = false;
            audioSourceScript.pitch -= Random.Range(-0.1f, 0.1f);
            audioSourceScript.PlayOneShot(clip);
            
            Destroy(audioSource, 0.5f);
        }
    }

    [System.Serializable]
    public class SoundData
    {
        public AudioClip AudioClip;
        public ESoundType SoundType;
    }
}