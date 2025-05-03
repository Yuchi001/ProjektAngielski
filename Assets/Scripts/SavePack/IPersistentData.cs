namespace SavePack
{
    public interface IPersistentData
    {
        public void OnLoadData(SaveManager.PlayerSaveData playerSaveData);

        public void OnSaveData(ref SaveManager.PlayerSaveData playerSaveData);
    }
}