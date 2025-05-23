namespace Meshes
{
    /// <summary>
    /// Enumeration describing the possible channels of a mesh.
    /// </summary>
    public enum MeshChannel : byte
    {
        /// <summary>
        /// Vertex positions channel.
        /// </summary>
        Position,

        /// <summary>
        /// UVs channel.
        /// </summary>
        UV,

        /// <summary>
        /// Normals channel.
        /// </summary>
        Normal,

        /// <summary>
        /// Tangents channel.
        /// </summary>
        Tangent,

        /// <summary>
        /// Bi-tangents channel.
        /// </summary>
        BiTangent,

        /// <summary>
        /// Vertex colors channel.
        /// </summary>
        Color
    }
}
