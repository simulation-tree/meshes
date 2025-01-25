using Meshes.Components;
using System;
using Worlds;

namespace Meshes
{
    /// <summary>
    /// An entity that stores mesh data.
    /// </summary>
    public readonly struct Mesh : IMesh
    {
        private readonly Entity entity;

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsMesh>();
            archetype.AddArrayElementType<MeshVertexIndex>();
        }

        public Mesh(World world, uint existingEntity)
        {
            entity = new(world, existingEntity);
        }

        /// <summary>
        /// Creates a blank mesh with no data/channels.
        /// </summary>
        public Mesh(World world)
        {
            entity = new(world);
            entity.AddComponent(new IsMesh());
            entity.CreateArray<MeshVertexIndex>();
        }

        /// <summary>
        /// Creates a mesh+request from an existing model and a sub mesh index.
        /// </summary>
        public Mesh(World world, Entity modelEntity, uint meshIndex = 0)
        {
            entity = new Entity<IsMeshRequest>(world, new IsMeshRequest((rint)1, meshIndex));
            entity.AddReference(modelEntity);
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public static implicit operator Entity(Mesh mesh)
        {
            return mesh.entity;
        }

        [Flags]
        public enum ChannelMask : byte
        {
            None = 0,
            Positions = 1,
            UVs = 2,
            Normals = 4,
            Tangents = 8,
            BiTangents = 16,
            Colors = 32
        }

        public enum Channel : byte
        {
            Position = 0,
            UV = 1,
            Normal = 2,
            Tangent = 3,
            BiTangent = 4,
            Color = 5
        }
    }
}
