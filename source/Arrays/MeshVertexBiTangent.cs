using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Array type for a single <see cref="Vector3"/> representing a bi-tangent vector.
    /// </summary>
    public readonly struct MeshVertexBiTangent
    {
        private readonly Vector3 value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexBiTangent(Vector3 value)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public static implicit operator Vector3(MeshVertexBiTangent meshVertexBiTangent)
        {
            return meshVertexBiTangent.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexBiTangent(Vector3 value)
        {
            return new MeshVertexBiTangent(value);
        }
    }
}
