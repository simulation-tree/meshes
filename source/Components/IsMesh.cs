using System;

namespace Meshes.Components
{
    public struct IsMesh : IEquatable<IsMesh>
    {
        /// <summary>
        /// Incremented when the entity has it's data updated (collections).
        /// </summary>
        public ushort version;

#if NET
        [Obsolete("Default constructor not supported", true)]
        public IsMesh()
        {
            throw new NotSupportedException("Default constructor not supported");
        }
#endif

        public IsMesh(ushort version)
        {
            this.version = version;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is IsMesh mesh && Equals(mesh);
        }

        public readonly bool Equals(IsMesh other)
        {
            return version == other.version;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(version);
        }

        public static bool operator ==(IsMesh left, IsMesh right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IsMesh left, IsMesh right)
        {
            return !(left == right);
        }
    }
}