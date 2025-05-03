using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using ItemPack.Enums;
using Managers.Enums;
using ProjectilePack;
using ProjectilePack.MovementStrategies;
using TargetSearchPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class ViolinLogic : ItemLogicBase
    {
        [SerializeField] private List<NotePair> noteList = new();
        
        private float Spread => GetStatValue(EItemSelfStatType.Spread);
        private float Scale => GetStatValue(EItemSelfStatType.ProjectileScale);
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileScale,
            EItemSelfStatType.Spread
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        private NearPlayerStrategy _findStrategy;
        private NearPlayerStrategy FindStrategy
        {
            get
            {
                return _findStrategy ??= new NearPlayerStrategy();
            }
        }
        
        protected override bool Use()
        {
            var target = TargetManager.FindTarget(FindStrategy);

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
                
                var direction = GetDirection(position);
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
                var projectileMovementStrategy = new DirectionMovementStrategy(direction, Speed);
                ProjectileManager.SpawnProjectile(projectileMovementStrategy, this)
                    .SetSprite(note.noteSprite, angle)
                    .SetFlip(flipX: direction.x < 0)
                    .SetScale(Scale)
                    .Ready();
                
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
            var direction = (pickedTargetPos - PlayerPos).normalized;
            var angleOffset = Random.Range(-Spread, Spread);
            var rotated = Quaternion.Euler(0, 0, angleOffset) * direction;
            return rotated.normalized;
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