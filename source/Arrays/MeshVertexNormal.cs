using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Array type for vertex normals.
    /// </summary>
    public readonly struct MeshVertexNormal
    {
        private readonly Vector3 value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexNormal(Vector3 value)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public static implicit operator Vector3(MeshVertexNormal vertexNormal)
        {
            return vertexNormal.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexNormal(Vector3 value)
        {
            return new MeshVertexNormal(value);
        }
    }
}