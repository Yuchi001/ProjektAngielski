using System.Collections.Generic;
using Managers;
using MapGeneratorPack;
using PlayerPack;
using PlayerPack.SO;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Character Structure", menuName = "Custom/Structure/Character")]
    public class SOCharacterStructure : SoStructure
    {
        [SerializeField] private SoCharacter character;
        [SerializeField] private RuntimeAnimatorController playerAnimController;
        
        public override bool OnInteract(StructureBase structureBase, IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            // TODO: Add transitions
            var newStructure = StructureGeneratorManager.GetStructure(a =>
            {
                if (a is SOCharacterStructure so) return so.character.ID == PlayerManager.Instance.PickedCharacter.ID;
                return false;
            });
            StructureGeneratorManager.SpawnStructure(newStructure, structureBase.transform.position);
            MissionManager.PickCharacter(character);
            Destroy(structureBase.gameObject);
            return true;
        }

        public override void OnSetup(StructureBase structureBase)
        {
            var animator = structureBase.AddComponent<Animator>();
            animator.runtimeAnimatorController = Instantiate(playerAnimController);
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, a.name == "DwarfAIdle" ? 
                    character.IdleAnimation : 
                    character.WalkingAnimation));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            animator.SetBool("isWalking", false);
        }

        #if UNITY_EDITOR
        
        /// <summary>
        /// EDITOR ONLY METHOD <param />
        /// Creates CharacterStructure asset based on given character;
        /// </summary>
        /// <param name="character">Character to create structure from.</param>
        public static void CreateCharacterStructureAsset(SoCharacter character)
        {
            var animController = Resources.Load<RuntimeAnimatorController>("Structures/Characters/CharacterAnim");
            
            var newCharacterStructureAsset = CreateInstance<SOCharacterStructure>();
            newCharacterStructureAsset.character = character;
            var structureName = $"{character.ID[0].ToString().ToUpper()}{character.ID.Substring(1)}Structure";
            newCharacterStructureAsset.bottomHoverMessage = "Switch";
            newCharacterStructureAsset.structureName = structureName;
            newCharacterStructureAsset.structureScale = 1f;
            newCharacterStructureAsset.structureSprite = character.CharacterSprite;
            newCharacterStructureAsset.playerAnimController = animController;
            
            const string folderPath = "Assets/Resources/Structures/Characters";
            var assetPath = Path.Combine(folderPath, $"{structureName}.asset");
            
            AssetDatabase.CreateAsset(newCharacterStructureAsset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Saved asset in: " + assetPath);
        }
        #endif
    }
}