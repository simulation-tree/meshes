using Data;
using Data.Components;
using Meshes.Components;
using Simulation;
using System;
using System.Diagnostics;
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

        public readonly USpan<uint> Indices => entity.GetArray<uint>();

        public readonly unsafe USpan<Vector3> Positions
        {
            get
            {
                ThrowIfMissingPositions();
                USpan<MeshVertexPosition> positions = entity.GetArray<MeshVertexPosition>();
                return new(positions.pointer, positions.Length);
            }
        }

        public readonly unsafe USpan<Vector2> UVs
        {
            get
            {
                ThrowIfMissingUVs();
                USpan<MeshVertexUV> uvs = entity.GetArray<MeshVertexUV>();
                return new(uvs.pointer, uvs.Length);
            }
        }

        public readonly unsafe USpan<Vector3> Normals
        {
            get
            {
                ThrowIfMissingNormals();
                USpan<MeshVertexNormal> normals = entity.GetArray<MeshVertexNormal>();
                return new(normals.pointer, normals.Length);
            }
        }

        public readonly unsafe USpan<Vector3> Tangents
        {
            get
            {
                ThrowIfMissingTangents();
                USpan<MeshVertexTangent> tangents = entity.GetArray<MeshVertexTangent>();
                return new(tangents.pointer, tangents.Length);
            }
        }

        public readonly unsafe USpan<Vector3> BiTangents
        {
            get
            {
                ThrowIfMissingBiTangents();
                USpan<MeshVertexBiTangent> biTangents = entity.GetArray<MeshVertexBiTangent>();
                return new(biTangents.pointer, biTangents.Length);
            }
        }

        public readonly unsafe USpan<Vector4> Colors
        {
            get
            {
                ThrowIfMissingColors();
                USpan<MeshVertexColor> colors = entity.GetArray<MeshVertexColor>();
                return new(colors.pointer, colors.Length);
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

        public readonly uint Version
        {
            get
            {
                IsMesh component = entity.GetComponentRef<IsMesh>();
                return component.version;
            }
        }

        public readonly (Vector3 min, Vector3 max) Bounds
        {
            get
            {
                ThrowIfMissingPositions();
                USpan<MeshVertexPosition> positions = entity.GetArray<MeshVertexPosition>();
                Vector3 min = new(float.MaxValue);
                Vector3 max = new(float.MinValue);
                for (uint i = 0; i < positions.Length; i++)
                {
                    Vector3 position = positions[i].value;
                    min = Vector3.Min(min, position);
                    max = Vector3.Max(max, position);
                }

                return (min, max);
            }
        }

        readonly uint IEntity.Value => entity.value;
        readonly World IEntity.World => entity.world;
        readonly Definition IEntity.Definition => new Definition().AddComponentType<IsMesh>().AddArrayType<uint>();

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
            entity.CreateArray<uint>();
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

        public readonly void IncrementVersion()
        {
            ref IsMesh mesh = ref entity.GetComponentRef<IsMesh>();
            mesh.version++;
        }

        public unsafe readonly USpan<Vector3> CreatePositions(uint length)
        {
            ThrowIfAlreadyContainsPositions();
            IncrementVersion();
            USpan<MeshVertexPosition> array = entity.CreateArray<MeshVertexPosition>(length);
            return new(array.pointer, length);
        }

        public unsafe readonly USpan<Vector2> CreateUVs(uint length)
        {
            ThrowIfAlreadyContainsUVs();
            IncrementVersion();
            USpan<MeshVertexUV> array = entity.CreateArray<MeshVertexUV>(length);
            return new(array.pointer, length);
        }

        public unsafe readonly USpan<Vector3> CreateNormals(uint length)
        {
            ThrowIfAlreadyContainsNormals();
            IncrementVersion();
            USpan<MeshVertexNormal> array = entity.CreateArray<MeshVertexNormal>(length);
            return new(array.pointer, length);
        }

        public unsafe readonly USpan<Vector3> CreateTangents(uint length)
        {
            ThrowIfAlreadyContainsTangents();
            IncrementVersion();
            USpan<MeshVertexTangent> array = entity.CreateArray<MeshVertexTangent>(length);
            return new(array.pointer, length);
        }

        public unsafe readonly USpan<Vector3> CreateBiTangents(uint length)
        {
            ThrowIfAlreadyContainsBiTangents();
            IncrementVersion();
            USpan<MeshVertexBiTangent> array = entity.CreateArray<MeshVertexBiTangent>(length);
            return new(array.pointer, length);
        }

        public unsafe readonly USpan<Color> CreateColors(uint length)
        {
            ThrowIfAlreadyContainsColors();
            IncrementVersion();
            USpan<MeshVertexColor> array = entity.CreateArray<MeshVertexColor>(length);
            return new(array.pointer, length);
        }

        public readonly void AddIndices(USpan<uint> indices)
        {
            uint count = entity.GetArrayLength<uint>();
            USpan<uint> span = entity.ResizeArray<uint>(count + indices.Length);
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
            USpan<Vector3> positions = default;
            USpan<Vector2> uvs = default;
            USpan<Vector3> normals = default;
            USpan<Vector3> tangents = default;
            USpan<Vector3> bitangents = default;
            USpan<Vector4> colors = default;

            static bool Contains(USpan<Channel> channels, Channel channel)
            {
                for (uint i = 0; i < channels.Length; i++)
                {
                    if (channels[i] == channel)
                    {
                        return true;
                    }
                }

                return false;
            }

            if (Contains(channels, Channel.Position))
            {
                positions = Positions;
            }

            if (Contains(channels, Channel.UV))
            {
                uvs = UVs;
            }

            if (Contains(channels, Channel.Normal))
            {
                normals = Normals;
            }

            if (Contains(channels, Channel.Tangent))
            {
                tangents = Tangents;
            }

            if (Contains(channels, Channel.BiTangent))
            {
                bitangents = BiTangents;
            }

            if (Contains(channels, Channel.Color))
            {
                colors = Colors;
            }

            uint vertexCount = VertexCount;
            for (uint i = 0; i < vertexCount; i++)
            {
                for (uint c = 0; c < channels.Length; c++)
                {
                    Channel channel = channels[c];
                    if (channel == Channel.Position)
                    {
                        Vector3 position = positions[i];
                        vertexData.Add(position.X);
                        vertexData.Add(position.Y);
                        vertexData.Add(position.Z);
                    }
                    else if (channel == Channel.UV)
                    {
                        Vector2 uv = uvs[i];
                        vertexData.Add(uv.X);
                        vertexData.Add(uv.Y);
                    }
                    else if (channel == Channel.Normal)
                    {
                        Vector3 normal = normals[i];
                        vertexData.Add(normal.X);
                        vertexData.Add(normal.Y);
                        vertexData.Add(normal.Z);
                    }
                    else if (channel == Channel.Tangent)
                    {
                        Vector3 tangent = tangents[i];
                        vertexData.Add(tangent.X);
                        vertexData.Add(tangent.Y);
                        vertexData.Add(tangent.Z);
                    }
                    else if (channel == Channel.BiTangent)
                    {
                        Vector3 bitangent = bitangents[i];
                        vertexData.Add(bitangent.X);
                        vertexData.Add(bitangent.Y);
                        vertexData.Add(bitangent.Z);
                    }
                    else if (channel == Channel.Color)
                    {
                        Vector4 color = colors[i];
                        vertexData.Add(color.X);
                        vertexData.Add(color.Y);
                        vertexData.Add(color.Z);
                        vertexData.Add(color.W);
                    }
                }
            }

            uint vertexSize = 0;
            for (uint c = 0; c < channels.Length; c++)
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

        [Conditional("DEBUG")]
        private readonly void ThrowIfAlreadyContainsPositions()
        {
            if (HasPositions)
            {
                throw new InvalidOperationException("Mesh already contains positions");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfAlreadyContainsUVs()
        {
            if (HasUVs)
            {
                throw new InvalidOperationException("Mesh already contains uvs");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfAlreadyContainsNormals()
        {
            if (HasNormals)
            {
                throw new InvalidOperationException("Mesh already contains normals");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfAlreadyContainsTangents()
        {
            if (HasTangents)
            {
                throw new InvalidOperationException("Mesh already contains tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfAlreadyContainsBiTangents()
        {
            if (HasBiTangents)
            {
                throw new InvalidOperationException("Mesh already contains bitangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfAlreadyContainsColors()
        {
            if (HasColors)
            {
                throw new InvalidOperationException("Mesh already contains colors");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfMissingPositions()
        {
            if (!HasPositions)
            {
                throw new InvalidOperationException("Mesh does not contain positions");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfMissingUVs()
        {
            if (!HasUVs)
            {
                throw new InvalidOperationException("Mesh does not contain uvs");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfMissingNormals()
        {
            if (!HasNormals)
            {
                throw new InvalidOperationException("Mesh does not contain normals");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfMissingTangents()
        {
            if (!HasTangents)
            {
                throw new InvalidOperationException("Mesh does not contain tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfMissingBiTangents()
        {
            if (!HasBiTangents)
            {
                throw new InvalidOperationException("Mesh does not contain bitangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfMissingColors()
        {
            if (!HasColors)
            {
                throw new InvalidOperationException("Mesh does not contain colors");
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
