using Worlds;

namespace Meshes.Components
{
    [Component]
    public struct IsMesh
    {
        /// <summary>
        /// Incremented when the entity has it's data updated (collections).
        /// </summary>
        public uint version;

        public IsMesh(uint version)
        {
            this.version = version;
        }
    }
}
