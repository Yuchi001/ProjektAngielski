using System.Collections.Generic;
using PlayerPack.SO;
using UnityEngine;

namespace Utils
{
    public static class AnimatorExtensions
    {
        private static RuntimeAnimatorController anim_controller;
        private static RuntimeAnimatorController ANIM_CONTROLLER
        {
            get
            {
                if (anim_controller == null) anim_controller = Resources.Load<RuntimeAnimatorController>("Structures/Characters/CharacterAnim");
                return anim_controller;
            } 
        }
        
        public static void SetCharacterAnimations(this Animator animator, SoCharacter character)
        {
            animator.runtimeAnimatorController = ANIM_CONTROLLER;
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, a.name == "DwarfAIdle" ? 
                    character.IdleAnimation : 
                    character.WalkingAnimation));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
        }
    }
}