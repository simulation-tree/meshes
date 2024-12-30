using System.Numerics;
using Worlds;

namespace Meshes
{
    [ArrayElement]
    public struct MeshVertexTangent
    {
        public Vector3 value;

        public MeshVertexTangent(Vector3 value)
        {
            this.value = value;
        }
    }
}
