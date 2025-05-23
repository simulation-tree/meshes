using System;

namespace Meshes.Components
{
    /// <summary>
    /// Component indicating that the entity is a mesh and contains channels of data.
    /// </summary>
    public struct IsMesh : IEquatable<IsMesh>
    {
        /// <summary>
        /// Incremented when the entity has it's data updated (collections).
        /// </summary>
        public ushort version;

        /// <summary>
        /// The channels of data that the mesh contains.
        /// </summary>
        public MeshChannelMask channels;

        /// <summary>
        /// Amount of vertices in the mesh.
        /// </summary>
        public int vertexCount;

        /// <summary>
        /// Amount of indices in the mesh.
        /// </summary>
        public int indexCount;

#if NET
        /// <inheritdoc/>
        [Obsolete("Default constructor not supported", true)]
        public IsMesh()
        {
            throw new NotSupportedException("Default constructor not supported");
        }
#endif

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public IsMesh(ushort version, MeshChannelMask channels, int vertexCount, int indexCount)
        {
            this.version = version;
            this.channels = channels;
            this.vertexCount = vertexCount;
            this.indexCount = indexCount;
        }

        /// <inheritdoc/>
        public readonly override bool Equals(object? obj)
        {
            return obj is IsMesh mesh && Equals(mesh);
        }

        /// <inheritdoc/>
        public readonly bool Equals(IsMesh other)
        {
            return version == other.version && channels == other.channels;
        }

        /// <inheritdoc/>
        public readonly override int GetHashCode()
        {
            return HashCode.Combine(version, channels);
        }

        /// <inheritdoc/>
        public static bool operator ==(IsMesh left, IsMesh right)
        {
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(IsMesh left, IsMesh right)
        {
            return !(left == right);
        }
    }
}