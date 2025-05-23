using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Array type the position of a vertex.
    /// </summary>
    public readonly struct MeshVertexPosition
    {
        private readonly Vector3 value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexPosition(Vector3 value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes the instance with the given <paramref name="x"/>, <paramref name="y"/>, and <paramref name="z"/> values.
        /// </summary>
        public MeshVertexPosition(float x, float y, float z)
        {
            value = new Vector3(x, y, z);
        }

        /// <inheritdoc/>
        public static implicit operator Vector3(MeshVertexPosition position)
        {
            return position.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexPosition(Vector3 position)
        {
            return new(position);
        }
    }
}