using Worlds;

namespace Meshes.Components
{
    /// <summary>
    /// Component indicating a request for mesh data out of a model.
    /// </summary>
    public struct IsMeshRequest
    {
        /// <summary>
        /// Reference to the model entity.
        /// </summary>
        public rint modelReference;

        /// <summary>
        /// Mesh index to load.
        /// </summary>
        public int meshIndex;

        /// <summary>
        /// Version of the request.
        /// </summary>
        public ushort version;

        /// <summary>
        /// Loaded state.
        /// </summary>
        public bool loaded;

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public IsMeshRequest(rint modelReference, int meshIndex)
        {
            version = 0;
            loaded = false;
            this.modelReference = modelReference;
            this.meshIndex = meshIndex;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public IsMeshRequest(ushort version, rint modelReference, int meshIndex, bool loaded)
        {
            this.version = version;
            this.modelReference = modelReference;
            this.meshIndex = meshIndex;
            this.loaded = loaded;
        }
    }
}