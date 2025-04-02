using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StructurePack
{
    public class StructureInteractionHandler
    {
        private readonly Dictionary<int, StructureBase> _validStructures = new();
        private readonly LinkedList<int> _queue = new();

        public int FocusedStructure => _queue.First?.Value ?? -1; 

        public void AddToQueue(StructureBase structure)
        {
            var instanceId = structure.GetInstanceID();
            if (!_validStructures.TryAdd(instanceId, structure)) return;

            _queue.AddFirst(instanceId);
        }

        public void RemoveFromQueue(int instanceId)
        {
            _validStructures.Remove(instanceId);
            _queue.Remove(instanceId);
        }

        public void HandleQueue()
        {
            // TODO: jesli gracz nie moze sie ruszac to nie powinien moc klikac
            HandleInteraction();
            HandleSwitch();
        }

        private void HandleInteraction()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            if (_queue.First == null) return;

            var peek = _queue.First.Value;
            _validStructures[peek].HandleInteraction();
        }

        private void HandleSwitch()
        {
            if (!Input.GetKeyDown(KeyCode.Tab)) return;

            if (_queue.First == null) return;

            var peek = _queue.First.Value;
            if (_validStructures[peek].Toggle) return;

            var old = _queue.First.Value;
            _queue.RemoveFirst();
            _queue.AddLast(old);
        }
    }
}