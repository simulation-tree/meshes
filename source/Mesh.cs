using Data;
using Data.Components;
using Meshes.Components;
using Simulation;
using System;
using System.Numerics;
using Unmanaged;
using Unmanaged.Collections;

namespace Meshes
{
    public readonly struct Mesh : IEntity
    {
        public readonly Entity entity;

        public readonly FixedString Name => entity.GetComponentRef<Name>().value;
        public readonly bool HasPositions => entity.ContainsArray<MeshVertexPosition>();
        public readonly bool HasUVs => entity.ContainsArray<MeshVertexUV>();
        public readonly bool HasNormals => entity.ContainsArray<MeshVertexNormal>();
        public readonly bool HasTangents => entity.ContainsArray<MeshVertexTangent>();
        public readonly bool HasBiTangents => entity.ContainsArray<MeshVertexBiTangent>();
        public readonly bool HasColors => entity.ContainsArray<MeshVertexColor>();

        public readonly uint VertexCount
        {
            get
            {
                bool hasPositions = HasPositions;
                if (!hasPositions)
                {
                    return 0;
                }

                return entity.GetArrayLength<MeshVertexPosition>();
            }
        }

        public readonly uint IndexCount => entity.GetArrayLength<uint>();

        public readonly unsafe Collection<uint> Indices
        {
            get
            {
                USpan<uint> indices = entity.GetArray<uint>();
                return new(indices.pointer, indices.length, RuntimeType.Get<uint>(), entity);
            }
        }

        public readonly unsafe Collection<Vector3> Positions
        {
            get
            {
                USpan<MeshVertexPosition> positions = entity.GetArray<MeshVertexPosition>();
                return new(positions.pointer, positions.length, RuntimeType.Get<MeshVertexPosition>(), entity);
            }
        }

        public unsafe Collection<Vector2> UVs
        {
            get
            {
                USpan<MeshVertexUV> uvs = entity.GetArray<MeshVertexUV>();
                return new(uvs.pointer, uvs.length, RuntimeType.Get<MeshVertexUV>(), entity);
            }
        }

        public unsafe Collection<Vector3> Normals
        {
            get
            {
                USpan<MeshVertexNormal> normals = entity.GetArray<MeshVertexNormal>();
                return new(normals.pointer, normals.length, RuntimeType.Get<MeshVertexNormal>(), entity);
            }
        }

        public readonly unsafe Collection<Vector3> Tangents
        {
            get
            {
                USpan<MeshVertexTangent> tangents = entity.GetArray<MeshVertexTangent>();
                return new(tangents.pointer, tangents.length, RuntimeType.Get<MeshVertexTangent>(), entity);
            }
        }

        public readonly unsafe Collection<Vector3> BiTangents
        {
            get
            {
                USpan<MeshVertexBiTangent> biTangents = entity.GetArray<MeshVertexBiTangent>();
                return new(biTangents.pointer, biTangents.length, RuntimeType.Get<MeshVertexBiTangent>(), entity);
            }
        }

        public readonly unsafe Collection<Vector4> Colors
        {
            get
            {
                USpan<MeshVertexColor> colors = entity.GetArray<MeshVertexColor>();
                return new(colors.pointer, colors.length, RuntimeType.Get<MeshVertexColor>(), entity);
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

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new([RuntimeType.Get<IsMesh>()], []);

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
            entity.CreateArray<uint>(0);
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

        public readonly override string ToString()
        {
            return entity.ToString();
        }

        public readonly uint GetVersion()
        {
            IsMesh component = entity.GetComponentRef<IsMesh>();
            return component.version;
        }

        public readonly (Vector3 min, Vector3 max) GetBounds()
        {
            bool hasPositions = HasPositions;
            if (hasPositions)
            {
                USpan<MeshVertexPosition> positions = entity.GetArray<MeshVertexPosition>();
                Vector3 min = new(float.MaxValue);
                Vector3 max = new(float.MinValue);
                for (uint i = 0; i < positions.length; i++)
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

            USpan<MeshVertexPosition> array = entity.CreateArray<MeshVertexPosition>(0);
            return new(array.pointer, 0, RuntimeType.Get<MeshVertexPosition>(), entity);
        }

        public unsafe readonly Collection<Vector2> CreateUVs()
        {
            if (HasUVs)
            {
                throw new InvalidOperationException("Mesh already contains uvs.");
            }

            USpan<MeshVertexUV> array = entity.CreateArray<MeshVertexUV>(0);
            return new(array.pointer, 0, RuntimeType.Get<MeshVertexUV>(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateNormals()
        {
            if (HasNormals)
            {
                throw new InvalidOperationException("Mesh already contains normals.");
            }

            USpan<MeshVertexNormal> array = entity.CreateArray<MeshVertexNormal>(0);
            return new(array.pointer, 0, RuntimeType.Get<MeshVertexNormal>(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateTangents()
        {
            if (HasTangents)
            {
                throw new InvalidOperationException("Mesh already contains tangents.");
            }

            USpan<MeshVertexTangent> array = entity.CreateArray<MeshVertexTangent>(0);
            return new(array.pointer, 0, RuntimeType.Get<MeshVertexTangent>(), entity);
        }

        public unsafe readonly Collection<Vector3> CreateBiTangents()
        {
            if (HasBiTangents)
            {
                throw new InvalidOperationException("Mesh already contains bitangents.");
            }

            USpan<MeshVertexBiTangent> array = entity.CreateArray<MeshVertexBiTangent>(0);
            return new(array.pointer, 0, RuntimeType.Get<MeshVertexBiTangent>(), entity);
        }

        public unsafe readonly Collection<Color> CreateColors()
        {
            if (HasColors)
            {
                throw new InvalidOperationException("Mesh already contains colors.");
            }

            USpan<MeshVertexColor> array = entity.CreateArray<MeshVertexColor>(0);
            return new(array.pointer, 0, RuntimeType.Get<MeshVertexColor>(), entity);
        }

        public readonly void AddIndices(USpan<uint> indices)
        {
            uint count = entity.GetArrayLength<uint>();
            USpan<uint> span = entity.ResizeArray<uint>(count + (uint)indices.length);
            indices.CopyTo(span.Slice(count));
        }

        public readonly void AddIndex(uint index)
        {
            uint count = entity.GetArrayLength<uint>();
            USpan<uint> span = entity.ResizeArray<uint>(count + 1);
            span[count] = index;
        }

        public readonly void AddTriangle(uint a, uint b, uint c)
        {
            uint count = entity.GetArrayLength<uint>();
            USpan<uint> span = entity.ResizeArray<uint>(count + 3);
            span[count] = a;
            span[count + 1] = b;
            span[count + 2] = c;
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
        public readonly uint Assemble(UnmanagedList<float> vertexData, USpan<Channel> channels)
        {
            USpan<MeshVertexPosition> positions = default;
            USpan<MeshVertexUV> uvs = default;
            USpan<MeshVertexNormal> normals = default;
            USpan<MeshVertexTangent> tangents = default;
            USpan<MeshVertexBiTangent> bitangents = default;
            USpan<MeshVertexColor> colors = default;

            static bool Contains(USpan<Channel> channels, Channel channel)
            {
                for (uint i = 0; i < channels.length; i++)
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
                if (HasPositions)
                {
                    positions = entity.GetArray<MeshVertexPosition>();
                }
            }

            if (Contains(channels, Channel.UV))
            {
                if (HasUVs)
                {
                    uvs = entity.GetArray<MeshVertexUV>();
                }
            }

            if (Contains(channels, Channel.Normal))
            {
                if (HasNormals)
                {
                    normals = entity.GetArray<MeshVertexNormal>();
                }
            }

            if (Contains(channels, Channel.Tangent))
            {
                if (HasTangents)
                {
                    tangents = entity.GetArray<MeshVertexTangent>();
                }
            }

            if (Contains(channels, Channel.BiTangent))
            {
                if (HasBiTangents)
                {
                    bitangents = entity.GetArray<MeshVertexBiTangent>();
                }
            }

            if (Contains(channels, Channel.Color))
            {
                if (HasColors)
                {
                    colors = entity.GetArray<MeshVertexColor>();
                }
            }

            uint vertexCount = VertexCount;
            for (uint i = 0; i < vertexCount; i++)
            {
                for (uint c = 0; c < channels.length; c++)
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
            for (uint c = 0; c < channels.length; c++)
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

        //todo: efficiency: this can be better optimized by batching modifications, then incrementing version when changes are submitted
        //rather than on every individual operation
        public unsafe struct Collection<T> where T : unmanaged, IEquatable<T>
        {
            private readonly uint entity;
            private readonly World world;
            private void* array;
            private uint length;
            private readonly RuntimeType arrayType;

            public readonly T this[uint index]
            {
                get
                {
                    return new USpan<T>(array, length)[index];
                }
                set
                {
                    new USpan<T>(array, length)[index] = value;
                    Modified();
                }
            }

            public readonly uint Count => length;

            internal unsafe Collection(void* array, uint length, RuntimeType arrayType, Entity entity)
            {
                this.array = array;
                this.length = length;
                this.entity = entity.value;
                this.world = entity.world;
                this.arrayType = arrayType;
            }

            public readonly USpan<T> AsSpan()
            {
                return new USpan<T>(array, length);
            }

            private unsafe readonly void Modified()
            {
                ref IsMesh mesh = ref world.GetComponentRef<IsMesh>(entity);
                mesh.version++;
            }

            public void Add(T item)
            {
                length++;
                array = world.ResizeArray(entity, arrayType, length);
                AsSpan()[length - 1] = item;
                Modified();
            }

            public void Clear()
            {
                length = 0;
                array = world.ResizeArray(entity, arrayType, length);
                Modified();
            }

            public readonly bool Contains(T item)
            {
                return AsSpan().Contains(item);
            }

            public readonly USpan<T>.Enumerator GetEnumerator()
            {
                return AsSpan().GetEnumerator();
            }

            public readonly uint IndexOf(T item)
            {
                return AsSpan().IndexOf(item);
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
