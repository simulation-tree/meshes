using Meshes.Components;
using Simulation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace Meshes
{
    public readonly struct Mesh : IDisposable
    {
        public readonly Entity entity;
        public readonly UnmanagedList<uint> indices;

        public readonly bool HasPositions => entity.ContainsCollection<MeshVertexPosition>();
        public readonly bool HasUVs => entity.ContainsCollection<MeshVertexUV>();
        public readonly bool HasNormals => entity.ContainsCollection<MeshVertexNormal>();
        public readonly bool HasTangents => entity.ContainsCollection<MeshVertexTangent>();
        public readonly bool HasBitangents => entity.ContainsCollection<MeshVertexBitangent>();
        public readonly bool HasColors => entity.ContainsCollection<MeshVertexColor>();

        private readonly ChannelMask Channels
        {
            get
            {
                ChannelMask channel = default;
                if (HasPositions)
                {
                    channel |= ChannelMask.Positions;
                }

                if (HasUVs)
                {
                    channel |= ChannelMask.UVs;
                }

                if (HasNormals)
                {
                    channel |= ChannelMask.Normals;
                }

                if (HasTangents)
                {
                    channel |= ChannelMask.Tangents;
                }

                if (HasBitangents)
                {
                    channel |= ChannelMask.Bitangents;
                }

                if (HasColors)
                {
                    channel |= ChannelMask.Colors;
                }

                return channel;
            }
        }

        public unsafe Collection<Vector3> Positions => new((UnsafeList*)entity.GetCollection<MeshVertexPosition>().AsPointer(), entity);
        public unsafe Collection<Vector2> UVs => new((UnsafeList*)entity.GetCollection<MeshVertexUV>().AsPointer(), entity);
        public unsafe Collection<Vector3> Normals => new((UnsafeList*)entity.GetCollection<MeshVertexNormal>().AsPointer(), entity);
        public unsafe Collection<Vector3> Tangents => new((UnsafeList*)entity.GetCollection<MeshVertexTangent>().AsPointer(), entity);
        public unsafe Collection<Vector3> Bitangents => new((UnsafeList*)entity.GetCollection<MeshVertexBitangent>().AsPointer(), entity);
        public unsafe Collection<Vector4> Colors => new((UnsafeList*)entity.GetCollection<MeshVertexColor>().AsPointer(), entity);

        public readonly uint VertexCount
        {
            get
            {
                if (HasPositions)
                {
                    return entity.GetCollection<MeshVertexPosition>().Count;
                }
                else return 0;
            }
        }

        public readonly (Vector3 min, Vector3 max) Bounds
        {
            get
            {
                if (HasPositions)
                {
                    UnmanagedList<MeshVertexPosition> positions = entity.GetCollection<MeshVertexPosition>();
                    Vector3 min = new(float.MaxValue);
                    Vector3 max = new(float.MinValue);
                    for (uint i = 0; i < positions.Count; i++)
                    {
                        Vector3 position = positions[i].value;
                        min = Vector3.Min(min, position);
                        max = Vector3.Max(max, position);
                    }

                    return (min, max);
                }
                else return default;
            }
        }

        public Mesh(World world, EntityID existingEntity)
        {
            this.entity = new(world, existingEntity);
            indices = this.entity.GetCollection<uint>();
        }

        public Mesh(World world)
        {
            entity = new(world);
            entity.AddComponent(new IsMesh());
            indices = entity.CreateCollection<uint>();
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public unsafe readonly Collection<Vector3> CreatePositions()
        {
            if (HasPositions)
            {
                throw new InvalidOperationException("Mesh already contains positions.");
            }

            UnmanagedList<MeshVertexPosition> list = entity.CreateCollection<MeshVertexPosition>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector2> CreateUVs()
        {
            if (HasUVs)
            {
                throw new InvalidOperationException("Mesh already contains texture coordinates.");
            }

            UnmanagedList<MeshVertexUV> list = entity.CreateCollection<MeshVertexUV>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateNormals()
        {
            if (HasNormals)
            {
                throw new InvalidOperationException("Mesh already contains normals.");
            }

            UnmanagedList<MeshVertexNormal> list = entity.CreateCollection<MeshVertexNormal>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateTangents()
        {
            if (HasTangents)
            {
                throw new InvalidOperationException("Mesh already contains tangents.");
            }

            UnmanagedList<MeshVertexTangent> list = entity.CreateCollection<MeshVertexTangent>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateBitangents()
        {
            if (HasBitangents)
            {
                throw new InvalidOperationException("Mesh already contains bitangents.");
            }

            UnmanagedList<MeshVertexBitangent> list = entity.CreateCollection<MeshVertexBitangent>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector4> CreateColors()
        {
            if (HasColors)
            {
                throw new InvalidOperationException("Mesh already contains colors.");
            }

            UnmanagedList<MeshVertexColor> list = entity.CreateCollection<MeshVertexColor>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public readonly void AddIndices(ReadOnlySpan<uint> indices)
        {
            this.indices.AddRange(indices);
        }

        public readonly void AddTriangle(uint a, uint b, uint c)
        {
            indices.Add(a);
            indices.Add(b);
            indices.Add(c);
        }

        /// <summary>
        /// Checks if the mesh contains all channels in the mask.
        /// </summary>
        public readonly bool ContainsChannel(ChannelMask mask)
        {
            if ((mask & ChannelMask.Positions) != 0 && !HasPositions)
            {
                return false;
            }
            else if ((mask & ChannelMask.UVs) != 0 && !HasUVs)
            {
                return false;
            }
            else if ((mask & ChannelMask.Normals) != 0 && !HasNormals)
            {
                return false;
            }
            else if ((mask & ChannelMask.Tangents) != 0 && !HasTangents)
            {
                return false;
            }
            else if ((mask & ChannelMask.Bitangents) != 0 && !HasBitangents)
            {
                return false;
            }
            else if ((mask & ChannelMask.Colors) != 0 && !HasColors)
            {
                return false;
            }

            return true;
        }

        public readonly bool ContainsChannel(Channel channel)
        {
            if (channel == Channel.Position && !HasPositions)
            {
                return false;
            }
            else if (channel == Channel.UV && !HasUVs)
            {
                return false;
            }
            else if (channel == Channel.Normal && !HasNormals)
            {
                return false;
            }
            else if (channel == Channel.Tangent && !HasTangents)
            {
                return false;
            }
            else if (channel == Channel.Bitangent && !HasBitangents)
            {
                return false;
            }
            else if (channel == Channel.Color && !HasColors)
            {
                return false;
            }

            return true;
        }

        public readonly uint Build(UnmanagedList<float> list)
        {
            return Build(list, Channels);
        }

        /// <summary>
        /// Appends all vertex data that fits the given channels mask.
        /// </summary>
        /// <returns>Amount of vertices appended.</returns>
        public readonly uint Build(UnmanagedList<float> list, ChannelMask mask)
        {
            uint vertexCount = VertexCount;
            UnmanagedList<MeshVertexPosition> positions = default;
            UnmanagedList<MeshVertexUV> uvs = default;
            UnmanagedList<MeshVertexNormal> normals = default;
            UnmanagedList<MeshVertexTangent> tangents = default;
            UnmanagedList<MeshVertexBitangent> bitangents = default;
            UnmanagedList<MeshVertexColor> colors = default;

            //throw if any channel is missing
            bool wantsPosition = (mask & ChannelMask.Positions) != 0;
            if (wantsPosition)
            {
                if (!HasPositions)
                {
                    throw new InvalidOperationException("Mesh does not contain positions.");
                }
                else
                {
                    positions = entity.GetCollection<MeshVertexPosition>();
                }
            }

            bool wantsUvs = (mask & ChannelMask.UVs) != 0;
            if (wantsUvs)
            {
                if (!HasUVs)
                {
                    throw new InvalidOperationException("Mesh does not contain texture coordinates.");
                }
                else
                {
                    uvs = entity.GetCollection<MeshVertexUV>();
                }
            }

            bool wantsNormals = (mask & ChannelMask.Normals) != 0;
            if (wantsNormals)
            {
                if (!HasNormals)
                {
                    throw new InvalidOperationException("Mesh does not contain normals.");
                }
                else
                {
                    normals = entity.GetCollection<MeshVertexNormal>();
                }
            }

            bool wantsTangents = (mask & ChannelMask.Tangents) != 0;
            if (wantsTangents)
            {
                if (!HasTangents)
                {
                    throw new InvalidOperationException("Mesh does not contain tangents.");
                }
                else
                {
                    tangents = entity.GetCollection<MeshVertexTangent>();
                }
            }

            bool wantsBitangents = (mask & ChannelMask.Bitangents) != 0;
            if (wantsBitangents)
            {
                if (!HasBitangents)
                {
                    throw new InvalidOperationException("Mesh does not contain bitangents.");
                }
                else
                {
                    bitangents = entity.GetCollection<MeshVertexBitangent>();
                }
            }

            bool wantsColors = (mask & ChannelMask.Colors) != 0;
            if (wantsColors)
            {
                if (!HasColors)
                {
                    throw new InvalidOperationException("Mesh does not contain colors.");
                }
                else
                {
                    colors = entity.GetCollection<MeshVertexColor>();
                }
            }

            for (uint i = 0; i < vertexCount; i++)
            {
                if (wantsPosition)
                {
                    Vector3 position = positions[i].value;
                    list.Add(position.X);
                    list.Add(position.Y);
                    list.Add(position.Z);
                }

                if (wantsUvs)
                {
                    Vector2 uv = uvs[i].value;
                    list.Add(uv.X);
                    list.Add(uv.Y);
                }

                if (wantsNormals)
                {
                    Vector3 normal = normals[i].value;
                    list.Add(normal.X);
                    list.Add(normal.Y);
                    list.Add(normal.Z);
                }

                if (wantsTangents)
                {
                    Vector3 tangent = tangents[i].value;
                    list.Add(tangent.X);
                    list.Add(tangent.Y);
                    list.Add(tangent.Z);
                }

                if (wantsBitangents)
                {
                    Vector3 bitangent = bitangents[i].value;
                    list.Add(bitangent.X);
                    list.Add(bitangent.Y);
                    list.Add(bitangent.Z);
                }

                if (wantsColors)
                {
                    Vector4 color = colors[i].value;
                    list.Add(color.X);
                    list.Add(color.Y);
                    list.Add(color.Z);
                    list.Add(color.W);
                }
            }

            return vertexCount;
        }

        public static RuntimeType GetCollectionType(Channel channel)
        {
            return channel switch
            {
                Channel.Position => RuntimeType.Get<Vector3>(),
                Channel.UV => RuntimeType.Get<Vector2>(),
                Channel.Normal => RuntimeType.Get<Vector3>(),
                Channel.Tangent => RuntimeType.Get<Vector3>(),
                Channel.Bitangent => RuntimeType.Get<Vector3>(),
                Channel.Color => RuntimeType.Get<Vector4>(),
                _ => throw new NotSupportedException($"Unsupported channel {channel}")
            };
        }

        public readonly struct Collection<T> : IList<T> where T : unmanaged, IEquatable<T>
        {
            private readonly UnmanagedList<T> list;
            private readonly Entity entity;

            public readonly T this[int index]
            {
                get => list[(uint)index];
                set => list[(uint)index] = value;
            }

            public readonly int Count => (int)list.Count;

            bool ICollection<T>.IsReadOnly => false;

            internal unsafe Collection(UnsafeList* list, Entity entity)
            {
                this.list = new(list);
                this.entity = entity;
            }

            private readonly void Modified()
            {
                ref IsMesh mesh = ref entity.GetComponentRef<IsMesh>();
                mesh.version++;
            }

            public readonly void Add(T item)
            {
                list.Add(item);
                Modified();
            }

            public readonly void Clear()
            {
                list.Clear();
                Modified();
            }

            public readonly bool Contains(T item)
            {
                return list.Contains(item);
            }

            void ICollection<T>.CopyTo(T[] array, int arrayIndex)
            {
                list.AsSpan().CopyTo(array.AsSpan(arrayIndex));
            }

            public readonly UnmanagedList<T>.Enumerator GetEnumerator()
            {
                return list.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public readonly int IndexOf(T item)
            {
                if (list.TryIndexOf(item, out uint index))
                {
                    return (int)index;
                }
                else return -1;
            }

            public readonly void Insert(int index, T item)
            {
                list.Insert((uint)index, item);
                Modified();
            }

            public readonly bool Remove(T item)
            {
                if (list.TryIndexOf(item, out uint index))
                {
                    list.RemoveAt(index);
                    Modified();
                    return true;
                }
                else return false;
            }

            public readonly void RemoveAt(int index)
            {
                list.RemoveAt((uint)index);
                Modified();
            }
        }

        [Flags]
        public enum ChannelMask : byte
        {
            Positions = 1,
            UVs = 2,
            Normals = 4,
            Tangents = 8,
            Bitangents = 16,
            Colors = 32
        }

        public enum Channel : byte
        {
            Position = 0,
            UV = 1,
            Normal = 2,
            Tangent = 3,
            Bitangent = 4,
            Color = 5
        }
    }
}
