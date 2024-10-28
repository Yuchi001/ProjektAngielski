using UnityEngine;

namespace Other
{
    public class SoEntityBase : ScriptableObject
    {
        public T As<T>() where T : SoEntityBase
        {
            return (T)this;
        }
    }
}