using Simulation;

namespace Meshes.Components
{
    public struct IsMeshRequest
    {
        public rint modelReference;
        public uint meshIndex;
        public uint version;

        public IsMeshRequest(rint modelReference, uint meshIndex)
        {
            version = 1;
            this.modelReference = modelReference;
            this.meshIndex = meshIndex;
        }
    }
}
