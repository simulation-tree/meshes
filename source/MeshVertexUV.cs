using System.Numerics;

namespace Meshes
{
    public struct MeshVertexUV
    {
        public Vector2 value;

        public MeshVertexUV(Vector2 value)
        {
            this.value = value;
        }

        public MeshVertexUV(float x, float y)
        {
            value = new(x, y);
        }
    }
}
