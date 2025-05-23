namespace Meshes
{
    /// <summary>
    /// Array type for <see cref="uint"/> mesh vertex indices.
    /// </summary>
    public readonly struct MeshVertexIndex
    {
        private readonly uint value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexIndex(uint value)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public static implicit operator uint(MeshVertexIndex index)
        {
            return index.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexIndex(uint index)
        {
            return new(index);
        }
    }
}