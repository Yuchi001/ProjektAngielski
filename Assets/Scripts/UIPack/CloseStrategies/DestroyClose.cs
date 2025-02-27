using Object = UnityEngine.Object;

namespace UIPack.CloseStrategies
{
    public class DestroyClose : ICloseStrategy
    {
        private readonly UIBase _uiBase;

        public DestroyClose(UIBase uiBase)
        {
            _uiBase = uiBase;
        }
        
        public void Close()
        {
            _uiBase.OnClose();
            _uiBase.Animator.SetTrigger("CLOSE");
            UIManager.RemoveUI(_uiBase);
            Object.Destroy(_uiBase, 1f);
        }
    }
}