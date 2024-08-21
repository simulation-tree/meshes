using System.Numerics;

namespace Meshes
{
    public struct MeshVertexPosition
    {
        public Vector3 value;

        public MeshVertexPosition(Vector3 value)
        {
            this.value = value;
        }

        public static implicit operator Vector3(MeshVertexPosition position) => position.value;
        public static implicit operator MeshVertexPosition(Vector3 position) => new(position);
    }
}
