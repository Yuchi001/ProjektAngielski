using System;
using StructurePack.SO;

namespace StructurePack
{
    public interface IStructure
    {
        public void Setup(SoStructure structureData, StructureBase structureBase);
    }
}