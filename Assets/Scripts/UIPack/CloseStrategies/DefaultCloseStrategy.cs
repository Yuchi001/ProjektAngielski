using System;
using UnityEngine;

namespace UIPack.CloseStrategies
{
    public class DefaultCloseStrategy : ICloseStrategy
    {
        public void Close(UIBase spawnedBase)
        {
            spawnedBase.OnClose();
        }
    }
}