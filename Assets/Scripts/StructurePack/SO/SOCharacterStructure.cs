using System.Collections;
using System.Collections.Generic;
using MainCameraPack;
using Managers;
using MapGeneratorPack;
using PlayerPack;
using PlayerPack.Enums;
using PlayerPack.SO;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

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

        public bool Is(SoCharacter c)
        {
            return c.ID == character.ID;
        }
        
        public bool Is(string id)
        {
            return id == character.ID;
        }
        
        public override bool OnInteract(StructureBase structureBase)
        {
            MainCamera.InOutAnim(0.6f, () =>
            {
                PlayerManager.LockKeys();
                var oldCharacter = PlayerManager.PickedCharacter;
                TavernManager.PickCharacter(character);
                
                var structureTransform = structureBase.transform;

                var playerPos = PlayerManager.PlayerPos;
                var structurePos = structureTransform.position;
            
                PlayerManager.SetPosition(structureTransform.GetChild(1).position);
                structureTransform.position = playerPos;
            
                var spriteTransform = structureBase.transform.GetChild(1);
                var animator = spriteTransform.GetComponent<Animator>();
                animator.SetCharacterAnimations(oldCharacter);
                animator.SetBool("isWalking", true);
                animator.speed = 0.5f;
                spriteTransform.rotation = new Quaternion(0, structurePos.x < playerPos.x ? 0 : 1, 0, 0);
                structureBase.StartCoroutine(GoBackToSpawn(oldCharacter, structureBase, structurePos));
            }, PlayerManager.UnlockKeys);
            return true;
        }

        public override void OnSetup(StructureBase structureBase)
        {
            var spriteTransform = structureBase.transform.GetChild(1);
            spriteTransform.rotation = new Quaternion(0,Random.Range(0, 2),0,0);
            var animator = spriteTransform.AddComponent<Animator>();
            animator.SetCharacterAnimations(character);
            animator.SetBool("isWalking", false);
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            var normalizedTime = Random.Range(0f, 1f) / stateInfo.length;
            animator.Play(stateInfo.fullPathHash, 0, normalizedTime);
            animator.speed = 0.5f;
        }

        private static IEnumerator GoBackToSpawn(SoCharacter asCharacter, StructureBase structureBase, Vector2 destination)
        {
            structureBase.SetCanInteract(false);
            var speed = asCharacter.StatDict[EPlayerStatType.MovementSpeed];
            while (Vector2.Distance(destination, structureBase.transform.position) > 0.01f)
            {
                structureBase.transform.position = Vector2.MoveTowards(structureBase.transform.position, destination,
                    Time.deltaTime * 0.5f);
                yield return new WaitForEndOfFrame();
            }
            
            var newStructure = StructureManager.GetStructure(a =>
            {
                if (a is SOCharacterStructure so) return so.character.ID == asCharacter.ID;
                return false;
            });
            StructureManager.SpawnStructure(newStructure, structureBase.transform.position);
            Destroy(structureBase.gameObject);
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