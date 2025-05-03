using System;
using UnityEngine;

namespace EnemyPack.States.StateData
{
    public abstract class StateDataBase : ScriptableObject
    {
        public T As<T>() where T : StateDataBase => (T)this;
        public bool Is<T>() => this is T;
    }
}