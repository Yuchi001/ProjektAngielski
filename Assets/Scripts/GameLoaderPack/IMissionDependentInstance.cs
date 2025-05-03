using MapPack;

namespace GameLoaderPack
{
    public interface IMissionDependentInstance
    {
        public void Init(MapManager.MissionData missionData);
    }
}