using System.Numerics;

namespace Meshes
{
    /// <summary>
    /// Array type for a single <see cref="Vector4"/> representing a color.
    /// </summary>
    public readonly struct MeshVertexColor
    {
        private readonly Vector4 value;

        /// <summary>
        /// Initializes the instance with the given <paramref name="value"/>.
        /// </summary>
        public MeshVertexColor(Vector4 value)
        {
            this.value = value;
        }

        /// <summary>
        /// Initializes the instance with the given <paramref name="r"/>, <paramref name="g"/>, <paramref name="b"/>, and <paramref name="a"/> values.
        /// </summary>
        public MeshVertexColor(float r, float g, float b, float a)
        {
            value = new Vector4(r, g, b, a);
        }

        /// <inheritdoc/>
        public static implicit operator Vector4(MeshVertexColor meshVertexColor)
        {
            return meshVertexColor.value;
        }

        /// <inheritdoc/>
        public static implicit operator MeshVertexColor(Vector4 value)
        {
            return new MeshVertexColor(value);
        }
    }
}