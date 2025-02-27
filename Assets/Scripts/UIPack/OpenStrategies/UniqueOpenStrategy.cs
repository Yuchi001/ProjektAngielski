using System;
using System.Linq;

namespace UIPack.OpenStrategies
{
    public class UniqueOpenStrategy<T> : IOpenStrategy where T: UIBase
    {
        private readonly Type _instanceType = typeof(T);
        private readonly UIBase _basePrefab;

        public UniqueOpenStrategy(UIBase basePrefab)
        {
            _basePrefab = basePrefab;
        }

        public bool Open(out UIBase uiBase)
        {
            var record = UIManager.GetCurrentUIBaseList().FirstOrDefault(ui => ui.GetType() == _instanceType);
            uiBase = record == default ? UIManager.SpawnUI(_basePrefab) : record.Script;
            
            uiBase.gameObject.SetActive(true);
            uiBase.Animator.SetTrigger("OPEN");
            
            uiBase.OnOpen();

            return true;
        }
    }
}