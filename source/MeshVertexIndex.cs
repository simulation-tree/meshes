namespace Meshes
{
    public struct MeshVertexIndex
    {
        public uint value;

        public MeshVertexIndex(uint value)
        {
            this.value = value;
        }

        public static implicit operator uint(MeshVertexIndex index) => index.value;
        public static implicit operator MeshVertexIndex(uint index) => new(index);
    }
}
