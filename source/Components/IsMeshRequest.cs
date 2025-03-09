using Worlds;

namespace Meshes.Components
{
    public struct IsMeshRequest
    {
        public rint modelReference;
        public int meshIndex;
        public uint version;
        public bool loaded;

        public IsMeshRequest(rint modelReference, int meshIndex)
        {
            version = 0;
            loaded = false;
            this.modelReference = modelReference;
            this.meshIndex = meshIndex;
        }

        public IsMeshRequest(uint version, rint modelReference, int meshIndex, bool loaded)
        {
            this.version = version;
            this.modelReference = modelReference;
            this.meshIndex = meshIndex;
            this.loaded = loaded;
        }

        public readonly IsMeshRequest BecomeLoaded()
        {
            return new IsMeshRequest(version, modelReference, meshIndex, true);
        }
    }
}