using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class DistanceTriggerStateData : StateDataBase
    {
        [SerializeField] private float triggerDistance;
        [SerializeField] private ETriggerType triggerType;

        public float TriggerDistance => triggerDistance;
        public ETriggerType TriggerType => triggerType;
            
        public enum ETriggerType
        {
            ENTER,
            EXIT
        }
    }
}