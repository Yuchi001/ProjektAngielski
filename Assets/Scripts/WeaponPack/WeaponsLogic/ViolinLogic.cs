using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Enums;
using UnityEngine;
using Utils;
using WeaponPack.Enums;
using WeaponPack.Other;

namespace WeaponPack.WeaponsLogic
{
    public class ViolinLogic : WeaponLogicBase
    {
        [SerializeField] private List<NotePair> noteList = new();
        [SerializeField] private GameObject projectilePrefab;
        
        private float Accuracy => GetStatValue(EWeaponStat.Accuracy) ?? 1;
        private float Scale => GetStatValue(EWeaponStat.ProjectileScale) ?? 1;
        
        protected override bool UseWeapon()
        {
            var target = UtilsMethods.FindTarget(transform.position);

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
                AudioManager.Instance.PlaySound(ESoundType.Note);

                var note = GetRandomNote(weightSum);
                
                var projectile = Instantiate(projectilePrefab, PlayerPos, Quaternion.identity);
                var projectileScript = projectile.GetComponent<Projectile>();
                
                projectileScript.Setup(Mathf.CeilToInt(Damage * note.damageMultiplier), Speed)
                    .SetDirection(GetDirection(position), checkX: true)
                    .SetLightColor(Color.clear)
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