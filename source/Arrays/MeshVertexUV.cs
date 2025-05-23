using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Array type for a single <see cref="Vector2"/> representing a texture coordinate.
    /// </summary>
    public readonly struct MeshVertexUV
    {
        private readonly Vector2 value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexUV(Vector2 value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes the instance with the given <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        public MeshVertexUV(float x, float y)
        {
            value = new(x, y);
        }

        /// <inheritdoc/>
        public static implicit operator Vector2(MeshVertexUV meshVertexUV)
        {
            return meshVertexUV.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexUV(Vector2 value)
        {
            return new MeshVertexUV(value);
        }
    }
}
