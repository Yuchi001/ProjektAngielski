namespace StructurePack
{
    public abstract class BaseStructureData<T> where T: class
    {
        public bool IsInitialized() => _initialized;
        private bool _initialized = false;

        public virtual T Init()
        {
            _initialized = true;
            return this as T;
        }
    }
}