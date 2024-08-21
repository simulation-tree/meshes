using Data;
using Data.Components;
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
    public readonly struct Mesh : IEntity, IDisposable
    {
        private readonly Entity entity;

        public readonly FixedString Name => entity.GetComponent<Name>().value;
        public readonly bool HasPositions => entity.ContainsList<MeshVertexPosition>();
        public readonly bool HasUVs => entity.ContainsList<MeshVertexUV>();
        public readonly bool HasNormals => entity.ContainsList<MeshVertexNormal>();
        public readonly bool HasTangents => entity.ContainsList<MeshVertexTangent>();
        public readonly bool HasBiTangents => entity.ContainsList<MeshVertexBiTangent>();
        public readonly bool HasColors => entity.ContainsList<MeshVertexColor>();

        public readonly uint VertexCount
        {
            get
            {
                bool hasPositions = HasPositions;
                if (!hasPositions)
                {
                    return 0;
                }

                return entity.GetList<MeshVertexPosition>().Count;
            }
        }

        public readonly uint IndexCount => entity.GetList<uint>().Count;

        public readonly unsafe Collection<uint> Indices
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<uint>().AsPointer();
                return new(list, entity);
            }
        }

        public readonly unsafe Collection<Vector3> Positions
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<MeshVertexPosition>().AsPointer();
                return new(list, entity);
            }
        }

        public unsafe Collection<Vector2> UVs
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<MeshVertexUV>().AsPointer();
                return new(list, entity);
            }
        }

        public unsafe Collection<Vector3> Normals
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<MeshVertexNormal>().AsPointer();
                return new(list, entity);
            }
        }

        public readonly unsafe Collection<Vector3> Tangents
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<MeshVertexTangent>().AsPointer();
                return new(list, entity);
            }
        }

        public readonly unsafe Collection<Vector3> BiTangents
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<MeshVertexBiTangent>().AsPointer();
                return new(list, entity);
            }
        }

        public readonly unsafe Collection<Vector4> Colors
        {
            get
            {
                UnsafeList* list = (UnsafeList*)entity.GetList<MeshVertexColor>().AsPointer();
                return new(list, entity);
            }
        }

        /// <summary>
        /// Retrieves the available channels on the mesh.
        /// </summary>
        public readonly ChannelMask Channels
        {
            get
            {
                bool hasPositions = HasPositions;
                bool hasUVs = HasUVs;
                bool hasNormals = HasNormals;
                bool hasTangents = HasTangents;
                bool hasBiTangents = HasBiTangents;
                bool hasColors = HasColors;
                ChannelMask mask = default;
                if (hasPositions)
                {
                    mask |= ChannelMask.Positions;
                }

                if (hasUVs)
                {
                    mask |= ChannelMask.UVs;
                }

                if (hasNormals)
                {
                    mask |= ChannelMask.Normals;
                }

                if (hasTangents)
                {
                    mask |= ChannelMask.Tangents;
                }

                if (hasBiTangents)
                {
                    mask |= ChannelMask.Tangents;
                }

                if (hasColors)
                {
                    mask |= ChannelMask.Colors;
                }

                return mask;
            }
        }

        eint IEntity.Value => entity.value;
        World IEntity.World => entity.world;

        public Mesh(World world, eint existingEntity)
        {
            this.entity = new(world, existingEntity);
        }

        /// <summary>
        /// Creates a blank mesh with no data/channels.
        /// </summary>
        public Mesh(World world)
        {
            entity = new(world);
            entity.AddComponent(new IsMesh());
            entity.CreateList<uint>();
        }

        /// <summary>
        /// Creates a mesh+request from an existing model and a sub mesh index.
        /// </summary>
        public Mesh(World world, Entity modelEntity, uint meshIndex = 0)
        {
            entity = new(world);
            rint modelReference = entity.AddReference(modelEntity);
            entity.AddComponent(new IsMeshRequest(modelReference, meshIndex));
        }

        public readonly void Dispose()
        {
            entity.Dispose();
        }

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        readonly Query IEntity.GetQuery(World world)
        {
            return new(world, RuntimeType.Get<IsMesh>());
        }

        public readonly uint GetVersion()
        {
            IsMesh component = entity.GetComponent<IsMesh>();
            return component.version;
        }

        public readonly (Vector3 min, Vector3 max) GetBounds()
        {
            bool hasPositions = HasPositions;
            if (hasPositions)
            {
                UnmanagedList<MeshVertexPosition> positions = entity.GetList<MeshVertexPosition>();
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

        public unsafe readonly Collection<Vector3> CreatePositions()
        {
            if (HasPositions)
            {
                throw new InvalidOperationException("Mesh already contains positions.");
            }

            UnmanagedList<MeshVertexPosition> list = entity.CreateList<MeshVertexPosition>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector2> CreateUVs()
        {
            if (HasUVs)
            {
                throw new InvalidOperationException("Mesh already contains uvs.");
            }

            UnmanagedList<MeshVertexUV> list = entity.CreateList<MeshVertexUV>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateNormals()
        {
            if (HasNormals)
            {
                throw new InvalidOperationException("Mesh already contains normals.");
            }

            UnmanagedList<MeshVertexNormal> list = entity.CreateList<MeshVertexNormal>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateTangents()
        {
            if (HasTangents)
            {
                throw new InvalidOperationException("Mesh already contains tangents.");
            }

            UnmanagedList<MeshVertexTangent> list = entity.CreateList<MeshVertexTangent>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateBiTangents()
        {
            if (HasBiTangents)
            {
                throw new InvalidOperationException("Mesh already contains bitangents.");
            }

            UnmanagedList<MeshVertexBiTangent> list = entity.CreateList<MeshVertexBiTangent>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public unsafe readonly Collection<Color> CreateColors()
        {
            if (HasColors)
            {
                throw new InvalidOperationException("Mesh already contains colors.");
            }

            UnmanagedList<MeshVertexColor> list = entity.CreateList<MeshVertexColor>();
            return new((UnsafeList*)list.AsPointer(), entity);
        }

        public readonly void AddIndices(ReadOnlySpan<uint> indices)
        {
            UnmanagedList<uint> list = entity.GetList<uint>();
            list.AddRange(indices);
        }

        public readonly void AddIndex(uint index)
        {
            UnmanagedList<uint> list = entity.GetList<uint>();
            list.Add(index);
        }

        public readonly void AddTriangle(uint a, uint b, uint c)
        {
            UnmanagedList<uint> list = entity.GetList<uint>();
            list.Add(a);
            list.Add(b);
            list.Add(c);
        }

        public readonly bool ContainsChannel(ChannelMask mask)
        {
            if ((mask & ChannelMask.Positions) != 0 && !HasPositions)
            {
                return false;
            }

            if ((mask & ChannelMask.UVs) != 0 && !HasUVs)
            {
                return false;
            }

            if ((mask & ChannelMask.Normals) != 0 && !HasNormals)
            {
                return false;
            }

            if ((mask & ChannelMask.Tangents) != 0 && !HasTangents)
            {
                return false;
            }

            if ((mask & ChannelMask.Bitangents) != 0 && !HasBiTangents)
            {
                return false;
            }

            if ((mask & ChannelMask.Colors) != 0 && !HasColors)
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
            else if (channel == Channel.BiTangent && !HasBiTangents)
            {
                return false;
            }
            else if (channel == Channel.Color && !HasColors)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Appends vertex data into the given list, in the order of the channels given.
        /// <para>If the mesh does not contain the data for a specific channel, it will use defaults.</para>
        /// </summary>
        /// <returns>How many <c>float</c> values compose a single vertex.</returns>
        public readonly uint Assemble(UnmanagedList<float> vertexData, ReadOnlySpan<Channel> channels)
        {
            UnmanagedList<MeshVertexPosition> positions = default;
            UnmanagedList<MeshVertexUV> uvs = default;
            UnmanagedList<MeshVertexNormal> normals = default;
            UnmanagedList<MeshVertexTangent> tangents = default;
            UnmanagedList<MeshVertexBiTangent> bitangents = default;
            UnmanagedList<MeshVertexColor> colors = default;
            bool disposePositions = false;
            bool disposeUVs = false;
            bool disposeNormals = false;
            bool disposeTangents = false;
            bool disposeBiTangents = false;
            bool disposeColors = false;
            static bool Contains(ReadOnlySpan<Channel> channels, Channel channel)
            {
                for (int i = 0; i < channels.Length; i++)
                {
                    if (channels[i] == channel)
                    {
                        return true;
                    }
                }

                return false;
            }

            //throw if any channel is missing
            if (Contains(channels, Channel.Position))
            {
                if (!HasPositions)
                {
                    positions = new();
                    disposePositions = true;
                }
                else
                {
                    positions = entity.GetList<MeshVertexPosition>();
                }
            }

            if (Contains(channels, Channel.UV))
            {
                if (!HasUVs)
                {
                    uvs = new();
                    disposeUVs = true;
                }
                else
                {
                    uvs = entity.GetList<MeshVertexUV>();
                }
            }

            if (Contains(channels, Channel.Normal))
            {
                if (!HasNormals)
                {
                    normals = new();
                    disposeNormals = true;
                }
                else
                {
                    normals = entity.GetList<MeshVertexNormal>();
                }
            }

            if (Contains(channels, Channel.Tangent))
            {
                if (!HasTangents)
                {
                    tangents = new();
                    disposeTangents = true;
                }
                else
                {
                    tangents = entity.GetList<MeshVertexTangent>();
                }
            }

            if (Contains(channels, Channel.BiTangent))
            {
                if (!HasBiTangents)
                {
                    bitangents = new();
                    disposeBiTangents = true;
                }
                else
                {
                    bitangents = entity.GetList<MeshVertexBiTangent>();
                }
            }

            if (Contains(channels, Channel.Color))
            {
                if (!HasColors)
                {
                    colors = new();
                    disposeColors = true;
                }
                else
                {
                    colors = entity.GetList<MeshVertexColor>();
                }
            }

            uint vertexCount = VertexCount;
            for (uint i = 0; i < vertexCount; i++)
            {
                for (int c = 0; c < channels.Length; c++)
                {
                    Channel channel = channels[c];
                    if (channel == Channel.Position)
                    {
                        Vector3 position = positions[i].value;
                        vertexData.Add(position.X);
                        vertexData.Add(position.Y);
                        vertexData.Add(position.Z);
                    }
                    else if (channel == Channel.UV)
                    {
                        Vector2 uv = uvs[i].value;
                        vertexData.Add(uv.X);
                        vertexData.Add(uv.Y);
                    }
                    else if (channel == Channel.Normal)
                    {
                        Vector3 normal = normals[i].value;
                        vertexData.Add(normal.X);
                        vertexData.Add(normal.Y);
                        vertexData.Add(normal.Z);
                    }
                    else if (channel == Channel.Tangent)
                    {
                        Vector3 tangent = tangents[i].value;
                        vertexData.Add(tangent.X);
                        vertexData.Add(tangent.Y);
                        vertexData.Add(tangent.Z);
                    }
                    else if (channel == Channel.BiTangent)
                    {
                        Vector3 bitangent = bitangents[i].value;
                        vertexData.Add(bitangent.X);
                        vertexData.Add(bitangent.Y);
                        vertexData.Add(bitangent.Z);
                    }
                    else if (channel == Channel.Color)
                    {
                        Vector4 color = colors[i].value;
                        vertexData.Add(color.X);
                        vertexData.Add(color.Y);
                        vertexData.Add(color.Z);
                        vertexData.Add(color.W);
                    }
                }
            }

            uint vertexSize = 0;
            for (int c = 0; c < channels.Length; c++)
            {
                Channel channel = channels[c];
                if (channel == Channel.Position)
                {
                    vertexSize += 3;
                }
                else if (channel == Channel.UV)
                {
                    vertexSize += 2;
                }
                else if (channel == Channel.Normal)
                {
                    vertexSize += 3;
                }
                else if (channel == Channel.Tangent)
                {
                    vertexSize += 3;
                }
                else if (channel == Channel.BiTangent)
                {
                    vertexSize += 3;
                }
                else if (channel == Channel.Color)
                {
                    vertexSize += 4;
                }
            }

            if (disposePositions)
            {
                positions.Dispose();
            }

            if (disposeUVs)
            {
                uvs.Dispose();
            }

            if (disposeNormals)
            {
                normals.Dispose();
            }

            if (disposeTangents)
            {
                tangents.Dispose();
            }

            if (disposeBiTangents)
            {
                bitangents.Dispose();
            }

            if (disposeColors)
            {
                colors.Dispose();
            }

            return vertexSize;
        }

        public static ChannelMask AddChannel(ref ChannelMask mask, Channel channel)
        {
            return channel switch
            {
                Channel.Position => mask |= ChannelMask.Positions,
                Channel.UV => mask |= ChannelMask.UVs,
                Channel.Normal => mask |= ChannelMask.Normals,
                Channel.Tangent => mask |= ChannelMask.Tangents,
                Channel.BiTangent => mask |= ChannelMask.Bitangents,
                Channel.Color => mask |= ChannelMask.Colors,
                _ => throw new NotSupportedException($"Unsupported channel {channel}")
            };
        }

        public static RuntimeType GetCollectionType(Channel channel)
        {
            return channel switch
            {
                Channel.Position => RuntimeType.Get<Vector3>(),
                Channel.UV => RuntimeType.Get<Vector2>(),
                Channel.Normal => RuntimeType.Get<Vector3>(),
                Channel.Tangent => RuntimeType.Get<Vector3>(),
                Channel.BiTangent => RuntimeType.Get<Vector3>(),
                Channel.Color => RuntimeType.Get<Vector4>(),
                _ => throw new NotSupportedException($"Unsupported channel {channel}")
            };
        }

        public static implicit operator Entity(Mesh mesh)
        {
            return mesh.entity;
        }

        //todo: efficiency: this can be better optimized by batching modifications, then incrementing version when changes are submitted
        //rather than on every individual operation
        public readonly struct Collection<T> : IList<T> where T : unmanaged, IEquatable<T>
        {
            private readonly UnmanagedList<T> list;
            private readonly nint component;

            public readonly T this[uint index]
            {
                get => list[index];
                set
                {
                    list[index] = value;
                    Modified();
                }
            }

            public readonly uint Count => list.Count;

            bool ICollection<T>.IsReadOnly => false;
            int ICollection<T>.Count => (int)list.Count;

            T IList<T>.this[int index]
            {
                get => list[(uint)index];
                set => list[(uint)index] = value;
            }

            internal unsafe Collection(UnsafeList* list, Entity entity)
            {
                this.list = new(list);
                ComponentChunk chunk = entity.world.GetComponentChunk(entity.value);
                component = chunk.GetComponentAddress<IsMesh>(entity.value);
            }

            public readonly ReadOnlySpan<T> AsSpan()
            {
                return list.AsSpan();
            }

            private unsafe readonly void Modified()
            {
                ref IsMesh mesh = ref System.Runtime.CompilerServices.Unsafe.AsRef<IsMesh>((void*)component);
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
            BiTangent = 4,
            Color = 5
        }
    }
}
