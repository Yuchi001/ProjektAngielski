using System;
using UnityEngine;

namespace UIPack.CloseStrategies
{
    public class DefaultClose : ICloseStrategy
    {
        private readonly UIBase _uiBase;
        
        public DefaultClose(UIBase uiBase)
        {
            _uiBase = uiBase;
        }
        
        public void Close()
        {
            _uiBase.OnClose();
            _uiBase.Animator.SetTrigger("CLOSE");
        }
    }
}