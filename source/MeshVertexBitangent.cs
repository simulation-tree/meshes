using System.Numerics;
using Worlds;

namespace Meshes
{
    [Array]
    public struct MeshVertexBiTangent
    {
        public Vector3 value;

        public MeshVertexBiTangent(Vector3 value)
        {
            this.value = value;
        }
    }
}
