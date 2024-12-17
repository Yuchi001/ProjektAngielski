using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using ItemPack.Enums;
using Managers;
using Managers.Enums;
using UnityEngine;
using Utils;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class ViolinLogic : ItemLogicBase
    {
        [SerializeField] private List<NotePair> noteList = new();
        
        private float Accuracy => GetStatValue(EItemSelfStatType.Spread) ?? 1;
        private float Scale => GetStatValue(EItemSelfStatType.ProjectileScale) ?? 1;
        
        protected override bool Use()
        {
            var target = UtilsMethods.FindNearestTarget(transform.position);

            if (target == null) return false;
            var position = target.transform.position;
            StartCoroutine(PlayAllNotes(position));
            return true;
        }

        private IEnumerator PlayAllNotes(Vector2 position)
        {
            var weightSum = noteList.Sum(n => n.weight);
            var randomNoteLength = Random.Range(1, ProjectileCount + 1);
            for (var i = 0; i < randomNoteLength; i++)
            {
                AudioManager.PlaySound(ESoundType.Note);

                var note = GetRandomNote(weightSum);
                
                var projectile = Instantiate(Projectile, PlayerPos, Quaternion.identity);
                
                projectile.Setup(Mathf.CeilToInt(Damage * note.damageMultiplier), Speed)
                    .SetDirection(GetDirection(position), checkX: true)
                    .SetSprite(note.noteSprite)
                    .SetSpriteRotation(90)
                    .SetScale(Scale)
                    .SetReady();
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        private NotePair GetRandomNote(int weightSum)
        {
            var randomWeight = Random.Range(0, weightSum + 1);
            foreach (var notePair in noteList)
            {
                if (randomWeight <= notePair.weight) return notePair;
                randomWeight -= notePair.weight;
            }
            return null;
        }

        private Vector2 GetDirection(Vector2 pickedTargetPos)
        {
            var pos = pickedTargetPos;
            pos.x += Random.Range(-Accuracy, Accuracy);
            pos.y += Random.Range(-Accuracy, Accuracy);
            return pos;
        }
    }

    [System.Serializable]
    public class NotePair
    {
        public Sprite noteSprite;
        public int weight = 100;
        public float damageMultiplier = 1;
    }
}