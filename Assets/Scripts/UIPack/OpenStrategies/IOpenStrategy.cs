namespace UIPack.OpenStrategies
{
    public interface IOpenStrategy
    {
        public bool Open(out UIBase uiBase, string key);

        public bool OpenAs<T>(out T uiBaseAs, string key) where T: UIBase
        {
            var result = Open(out var uiBase, key);
            uiBaseAs = (T)uiBase;
            return result;
        }
    }
}