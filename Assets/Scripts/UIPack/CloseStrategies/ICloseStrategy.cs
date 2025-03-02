using System;

namespace UIPack.CloseStrategies
{
    public interface ICloseStrategy
    {
        public void Close(UIBase spawnedBase);
    }
}