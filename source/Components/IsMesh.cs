using Worlds;

namespace Meshes.Components
{
    [Component]
    public readonly struct IsMesh
    {
        /// <summary>
        /// Incremented when the entity has it's data updated (collections).
        /// </summary>
        public readonly uint version;

        public IsMesh(uint version)
        {
            this.version = version;
        }

        public readonly IsMesh IncrementVersion()
        {
            return new IsMesh(version + 1);
        }
    }
}
