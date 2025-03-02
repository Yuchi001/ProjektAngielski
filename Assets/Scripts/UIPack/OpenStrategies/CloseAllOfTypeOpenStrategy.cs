using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UIPack.OpenStrategies
{
    public class CloseAllOfTypeOpenStrategy<T> : IOpenStrategy
    {
        private readonly Type _instanceType = typeof(T);
        private readonly UIBase _basePrefab;
        private readonly bool _alwaysNew;

        public CloseAllOfTypeOpenStrategy(UIBase basePrefab, bool alwaysNew)
        {
            _basePrefab = basePrefab;
            _alwaysNew = alwaysNew;
        }

        public bool Open(out UIBase uiBase, string key)
        {
            foreach (var r in  new List<UIManager.UIRecord>(UIManager.GetCurrentUIBaseList()))
            {
                if (!_instanceType.IsInstanceOfType(r.Script) || r.Script == null) continue;
                UIManager.CloseUI(r.Key);
            }
            
            if (_alwaysNew)
            {
                uiBase = UIManager.SpawnUI(_basePrefab);
                uiBase.OnOpen(key);
                return true;
            }

            var record = UIManager.GetCurrentUIBaseList().FirstOrDefault(r => r.Key == key);
            uiBase = record == default ? UIManager.SpawnUI(_basePrefab) : record.Script;
            uiBase.OnOpen(key);

            return true;
        }
    }
}