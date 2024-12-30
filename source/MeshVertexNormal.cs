using System.Numerics;
using Worlds;

namespace Meshes
{
    [ArrayElement]
    public struct MeshVertexNormal
    {
        public Vector3 value;

        public MeshVertexNormal(Vector3 value)
        {
            this.value = value;
        }
    }
}
