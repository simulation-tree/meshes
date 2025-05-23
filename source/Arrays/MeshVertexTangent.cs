using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Array type for a single <see cref="Vector3"/> representing a tangent vector.
    /// </summary>
    public readonly struct MeshVertexTangent
    {
        private readonly Vector3 value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexTangent(Vector3 value)
        {
            this.value = value;
        }

        /// <inheritdoc/>
        public static implicit operator Vector3(MeshVertexTangent meshVertexTangent)
        {
            return meshVertexTangent.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexTangent(Vector3 value)
        {
            return new MeshVertexTangent(value);
        }
    }
}