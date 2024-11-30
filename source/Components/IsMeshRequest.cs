using Worlds;

namespace Meshes.Components
{
    [Component]
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
