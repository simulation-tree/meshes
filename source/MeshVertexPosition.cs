using System.Numerics;
using Worlds;

namespace Meshes
{
    [Array]
    public struct MeshVertexPosition
    {
        public Vector3 value;

        public MeshVertexPosition(Vector3 value)
        {
            this.value = value;
        }

        public MeshVertexPosition(float x, float y, float z)
        {
            value = new Vector3(x, y, z);
        }

        public static implicit operator Vector3(MeshVertexPosition position) => position.value;
        public static implicit operator MeshVertexPosition(Vector3 position) => new(position);
    }
}
