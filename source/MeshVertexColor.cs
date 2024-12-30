using System.Numerics;
using Worlds;

namespace Meshes
{
    [ArrayElement]
    public struct MeshVertexColor
    {
        public Vector4 value;

        public MeshVertexColor(Vector4 value)
        {
            this.value = value;
        }
    }
}
