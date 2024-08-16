namespace Meshes.Components
{
    public struct IsMeshRequest
    {
        public uint meshIndex;
        public bool changed;

        public IsMeshRequest(uint meshIndex)
        {
            this.meshIndex = meshIndex;
            changed = true;
        }
    }
}
