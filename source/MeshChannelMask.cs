using System;

namespace Meshes
{
    /// <summary>
    /// Mask representing the possible channels of a mesh.
    /// </summary>
    [Flags]
    public enum MeshChannelMask : byte
    {
        /// <summary>
        /// Empty.
        /// </summary>
        None = 0,

        /// <summary>
        /// Vertex positions channel.
        /// </summary>
        Positions = 1,

        /// <summary>
        /// Vertex UVs channel.
        /// </summary>
        UVs = 2,

        /// <summary>
        /// Normals channel.
        /// </summary>
        Normals = 4,

        /// <summary>
        /// Tangents channel.
        /// </summary>
        Tangents = 8,

        /// <summary>
        /// Bi-tangents channel.
        /// </summary>
        BiTangents = 16,

        /// <summary>
        /// Vertex colors channel.
        /// </summary>
        Colors = 32
    }
}
