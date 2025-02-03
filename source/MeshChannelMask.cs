using System;

namespace Meshes
{
    [Flags]
    public enum MeshChannelMask : byte
    {
        None = 0,
        Positions = 1,
        UVs = 2,
        Normals = 4,
        Tangents = 8,
        BiTangents = 16,
        Colors = 32
    }
}
