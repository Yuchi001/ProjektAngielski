using System;
using System.Collections.Generic;
using PlayerPack.SO;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
        
        public static PlayerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private Animator animator;
        
        private SoCharacter _pickedCharacter;

        public SoCharacter PickedCharacter => _pickedCharacter;

        public PlayerExp PlayerExp => GetComponent<PlayerExp>();
        public PlayerHealth PlayerHealth => GetComponent<PlayerHealth>();
        public PlayerWeaponry PlayerWeaponry => GetComponent<PlayerWeaponry>();

        public void Setup(SoCharacter pickedCharacter)
        {
            _pickedCharacter = pickedCharacter;
            playerSpriteRenderer.sprite = _pickedCharacter.CharacterSprite;

            foreach (var mono in GetComponentsInChildren<MonoBehaviour>())
                mono.enabled = true;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, a.name == "DwarfAIdle" ? 
                    PickedCharacter.IdleAnimation : 
                    PickedCharacter.WalkingAnimation));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            GetComponent<PlayerWeaponry>().AddWeapon(pickedCharacter.StartingWeapon);
        }
    }
}