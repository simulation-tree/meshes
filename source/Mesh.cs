using Meshes.Components;
using System;
using System.Diagnostics;
using System.Numerics;
using Unmanaged;
using Worlds;

namespace Meshes
{
    /// <summary>
    /// An entity that stores mesh data.
    /// </summary>
    public readonly partial struct Mesh : IEntity
    {
        public readonly bool IsLoaded
        {
            get
            {
                if (TryGetComponent(out IsMeshRequest request))
                {
                    return request.loaded;
                }

                return IsCompliant;
            }
        }

        public readonly bool ContainsPositions => ContainsArray<MeshVertexPosition>();
        public readonly bool ContainsUVs => ContainsArray<MeshVertexUV>();
        public readonly bool ContainsNormals => ContainsArray<MeshVertexNormal>();
        public readonly bool ContainsTangents => ContainsArray<MeshVertexTangent>();
        public readonly bool ContainsBiTangents => ContainsArray<MeshVertexBiTangent>();
        public readonly bool ContainsColors => ContainsArray<MeshVertexColor>();

        public readonly uint VertexCount
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                return GetArrayLength<MeshVertexPosition>();
            }
        }

        public readonly uint IndexCount
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArrayLength<MeshVertexIndex>();
            }
        }

        public readonly USpan<Vector3> Positions
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                return GetArray<MeshVertexPosition>().As<Vector3>();
            }
        }

        public readonly USpan<Vector2> UVs
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfUVsMissing();

                return GetArray<MeshVertexUV>().As<Vector2>();
            }
        }

        public readonly USpan<Vector3> Normals
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfNormalsMissing();

                return GetArray<MeshVertexNormal>().As<Vector3>();
            }
        }

        public readonly USpan<Vector3> Tangents
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfTangentsMissing();

                return GetArray<MeshVertexTangent>().As<Vector3>();
            }
        }

        public readonly USpan<Vector3> BiTangents
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfBiTangentsMissing();

                return GetArray<MeshVertexBiTangent>().As<Vector3>();
            }
        }

        public readonly USpan<Vector4> Colors
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfColorsMissing();

                return GetArray<MeshVertexColor>().As<Vector4>();
            }
        }

        public readonly USpan<uint> Indices
        {
            get
            {
                ThrowIfNotLoaded();

                return GetArray<MeshVertexIndex>().As<uint>();
            }
        }

        public readonly MeshChannelMask Channels
        {
            get
            {
                ThrowIfNotLoaded();

                MeshChannelMask mask = 0;
                if (ContainsPositions) mask |= MeshChannelMask.Positions;
                if (ContainsUVs) mask |= MeshChannelMask.UVs;
                if (ContainsNormals) mask |= MeshChannelMask.Normals;
                if (ContainsTangents) mask |= MeshChannelMask.Tangents;
                if (ContainsBiTangents) mask |= MeshChannelMask.BiTangents;
                if (ContainsColors) mask |= MeshChannelMask.Colors;
                return mask;
            }
        }

        public readonly uint VertexSize
        {
            get
            {
                ThrowIfNotLoaded();

                return Channels.GetVertexSize();
            }
        }

        public readonly (Vector3 min, Vector3 max) Bounds
        {
            get
            {
                ThrowIfNotLoaded();
                ThrowIfPositionsMissing();

                USpan<Vector3> positions = Positions;
                Vector3 min = new(float.MaxValue);
                Vector3 max = new(float.MinValue);
                for (uint i = 0; i < positions.Length; i++)
                {
                    Vector3 position = positions[i];
                    min = Vector3.Min(min, position);
                    max = Vector3.Max(max, position);
                }

                return (min, max);
            }
        }

        public readonly uint Version => GetComponent<IsMesh>().version;

        /// <summary>
        /// Creates a blank mesh with no data/channels.
        /// </summary>
        public Mesh(World world)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray<MeshVertexIndex>();
        }

        /// <summary>
        /// Creates a mesh+request from an existing model and a sub mesh index.
        /// </summary>
        public Mesh(World world, Entity modelEntity, uint meshIndex = 0)
        {
            this.world = world;
            value = world.CreateEntity(new IsMeshRequest((rint)1, meshIndex));
            AddReference(modelEntity);
        }

        public Mesh(World world, USpan<Vector3> positions, USpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<MeshVertexPosition>());
            CreateArray(indices.As<MeshVertexIndex>());
        }

        public Mesh(World world, USpan<Vector3> positions, USpan<Vector2> uvs, USpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<MeshVertexPosition>());
            CreateArray(uvs.As<MeshVertexUV>());
            CreateArray(indices.As<MeshVertexIndex>());
        }

        public Mesh(World world, USpan<Vector3> positions, USpan<Vector2> uvs, USpan<Vector3> normals, USpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<MeshVertexPosition>());
            CreateArray(uvs.As<MeshVertexUV>());
            CreateArray(normals.As<MeshVertexNormal>());
            CreateArray(indices.As<MeshVertexIndex>());
        }

        public Mesh(World world, USpan<Vector3> positions, USpan<Vector2> uvs, USpan<Vector4> colors, USpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<MeshVertexPosition>());
            CreateArray(uvs.As<MeshVertexUV>());
            CreateArray(colors.As<MeshVertexColor>());
            CreateArray(indices.As<MeshVertexIndex>());
        }

        public Mesh(World world, USpan<Vector3> positions, USpan<Vector2> uvs, USpan<Vector3> normals, USpan<Vector4> colors, USpan<uint> indices)
        {
            this.world = world;
            value = world.CreateEntity(new IsMesh(1));
            CreateArray(positions.As<MeshVertexPosition>());
            CreateArray(uvs.As<MeshVertexUV>());
            CreateArray(normals.As<MeshVertexNormal>());
            CreateArray(colors.As<MeshVertexColor>());
            CreateArray(indices.As<MeshVertexIndex>());
        }

        readonly void IEntity.Describe(ref Archetype archetype)
        {
            archetype.AddComponentType<IsMesh>();
            archetype.AddArrayType<MeshVertexIndex>();
        }

        public readonly override string ToString()
        {
            return value.ToString();
        }

        public readonly bool ContainsChannel(MeshChannel channel)
        {
            return channel switch
            {
                MeshChannel.Position => ContainsPositions,
                MeshChannel.UV => ContainsUVs,
                MeshChannel.Normal => ContainsNormals,
                MeshChannel.Tangent => ContainsTangents,
                MeshChannel.BiTangent => ContainsBiTangents,
                MeshChannel.Color => ContainsColors,
                _ => false,
            };
        }

        /// <summary>
        /// Appends vertex data into the given span, in the order of the channels given.
        /// <para>Missing channels on the mesh are skipped.</para>
        /// </summary>
        /// <returns>How many <see cref="float"/>s were added to <paramref name="vertexData"/>.</returns>
        public readonly uint Assemble(USpan<float> vertexData, USpan<MeshChannel> channels)
        {
            USpan<Vector3> positions = default;
            USpan<Vector2> uvs = default;
            USpan<Vector3> normals = default;
            USpan<Vector3> tangents = default;
            USpan<Vector3> biTangents = default;
            USpan<Vector4> colors = default;

            for (uint i = 0; i < channels.Length; i++)
            {
                MeshChannel channel = channels[i];
                if (channel == MeshChannel.Position)
                {
                    positions = Positions;
                }
                else if (channel == MeshChannel.UV)
                {
                    uvs = UVs;
                }
                else if (channel == MeshChannel.Normal)
                {
                    normals = Normals;
                }
                else if (channel == MeshChannel.Tangent)
                {
                    tangents = Tangents;
                }
                else if (channel == MeshChannel.BiTangent)
                {
                    biTangents = BiTangents;
                }
                else if (channel == MeshChannel.Color)
                {
                    colors = Colors;
                }
            }

            uint vertexCount = Positions.Length;
            uint index = 0;
            for (uint i = 0; i < vertexCount; i++)
            {
                for (uint c = 0; c < channels.Length; c++)
                {
                    MeshChannel channel = channels[c];
                    if (channel == MeshChannel.Position)
                    {
                        Vector3 position = positions[i];
                        vertexData[index++] = position.X;
                        vertexData[index++] = position.Y;
                        vertexData[index++] = position.Z;
                    }
                    else if (channel == MeshChannel.UV)
                    {
                        Vector2 uv = uvs[i];
                        vertexData[index++] = uv.X;
                        vertexData[index++] = uv.Y;
                    }
                    else if (channel == MeshChannel.Normal)
                    {
                        Vector3 normal = normals[i];
                        vertexData[index++] = normal.X;
                        vertexData[index++] = normal.Y;
                        vertexData[index++] = normal.Z;
                    }
                    else if (channel == MeshChannel.Tangent)
                    {
                        Vector3 tangent = tangents[i];
                        vertexData[index++] = tangent.X;
                        vertexData[index++] = tangent.Y;
                        vertexData[index++] = tangent.Z;
                    }
                    else if (channel == MeshChannel.BiTangent)
                    {
                        Vector3 bitangent = biTangents[i];
                        vertexData[index++] = bitangent.X;
                        vertexData[index++] = bitangent.Y;
                        vertexData[index++] = bitangent.Z;
                    }
                    else if (channel == MeshChannel.Color)
                    {
                        Vector4 color = colors[i];
                        vertexData[index++] = color.X;
                        vertexData[index++] = color.Y;
                        vertexData[index++] = color.Z;
                        vertexData[index++] = color.W;
                    }
                }
            }

            return index;
        }

        public readonly void IncrementVersion()
        {
            ThrowIfNotLoaded();

            ref IsMesh mesh = ref GetComponent<IsMesh>();
            mesh = mesh.IncrementVersion();
        }

        public readonly USpan<Vector3> CreatePositions(uint length)
        {
            ThrowIfPositionsPresent();

            IncrementVersion();
            return CreateArray<MeshVertexPosition>(length).As<Vector3>();
        }

        public readonly USpan<Vector2> CreateUVs(uint length)
        {
            ThrowIfUVsPresent();

            IncrementVersion();
            return CreateArray<MeshVertexUV>(length).As<Vector2>();
        }

        public readonly USpan<Vector3> CreateNormals(uint length)
        {
            ThrowIfNormalsPresent();

            IncrementVersion();
            return CreateArray<MeshVertexNormal>(length).As<Vector3>();
        }

        public readonly USpan<Vector3> CreateTangents(uint length)
        {
            ThrowIfTangentsPresent();

            IncrementVersion();
            return CreateArray<MeshVertexTangent>(length).As<Vector3>();
        }

        public readonly USpan<Vector3> CreateBiTangents(uint length)
        {
            ThrowIfBiTangentsPresent();

            IncrementVersion();
            return CreateArray<MeshVertexBiTangent>(length).As<Vector3>();
        }

        public readonly USpan<Vector4> CreateColors(uint length)
        {
            ThrowIfColorsPresent();

            IncrementVersion();
            return CreateArray<MeshVertexColor>(length).As<Vector4>();
        }

        public readonly USpan<Vector3> ResizePositions(uint newLength)
        {
            ThrowIfPositionsMissing();

            IncrementVersion();
            return ResizeArray<MeshVertexPosition>(newLength).As<Vector3>();
        }

        public readonly USpan<Vector2> ResizeUVs(uint newLength)
        {
            ThrowIfUVsMissing();

            IncrementVersion();
            return ResizeArray<MeshVertexUV>(newLength).As<Vector2>();
        }

        public readonly USpan<Vector3> ResizeNormals(uint newLength)
        {
            ThrowIfNormalsMissing();

            IncrementVersion();
            return ResizeArray<MeshVertexNormal>(newLength).As<Vector3>();
        }

        public readonly USpan<Vector3> ResizeTangents(uint newLength)
        {
            ThrowIfTangentsMissing();

            IncrementVersion();
            return ResizeArray<MeshVertexTangent>(newLength).As<Vector3>();
        }

        public readonly USpan<Vector3> ResizeBiTangents(uint newLength)
        {
            ThrowIfBiTangentsMissing();

            IncrementVersion();
            return ResizeArray<MeshVertexBiTangent>(newLength).As<Vector3>();
        }

        public readonly USpan<Vector4> ResizeColors(uint newLength)
        {
            ThrowIfColorsMissing();

            IncrementVersion();
            return ResizeArray<MeshVertexColor>(newLength).As<Vector4>();
        }

        public readonly USpan<uint> ResizeIndices(uint newLength)
        {
            IncrementVersion();

            return ResizeArray<MeshVertexIndex>(newLength).As<uint>();
        }

        public readonly void AddIndices(USpan<uint> indices)
        {
            IncrementVersion();

            uint length = GetArrayLength<MeshVertexIndex>();
            USpan<uint> array = ResizeArray<MeshVertexIndex>(length + indices.Length).As<uint>();
            indices.CopyTo(array.Slice(length));
        }

        public readonly void AddIndex(uint index)
        {
            IncrementVersion();

            uint length = GetArrayLength<MeshVertexIndex>();
            ResizeArray<MeshVertexIndex>(length + 1)[length] = index;
        }

        public readonly void AddTriangle(uint a, uint b, uint c)
        {
            IncrementVersion();

            uint length = GetArrayLength<MeshVertexIndex>();
            USpan<uint> array = ResizeArray<MeshVertexIndex>(length + 3).As<uint>();
            array[length] = a;
            array[length + 1] = b;
            array[length + 2] = c;
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfNotLoaded()
        {
            if (!IsLoaded)
            {
                throw new InvalidOperationException($"Mesh `{value}` is not loaded");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPositionsMissing()
        {
            if (!ContainsPositions)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain positions");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfPositionsPresent()
        {
            if (ContainsPositions)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains positions");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfUVsMissing()
        {
            if (!ContainsUVs)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain UVs");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfUVsPresent()
        {
            if (ContainsUVs)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains UVs");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfNormalsMissing()
        {
            if (!ContainsNormals)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain normals");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfNormalsPresent()
        {
            if (ContainsNormals)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains normals");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfTangentsMissing()
        {
            if (!ContainsTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfTangentsPresent()
        {
            if (ContainsTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfBiTangentsMissing()
        {
            if (!ContainsBiTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain bi-tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfBiTangentsPresent()
        {
            if (ContainsBiTangents)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains bi-tangents");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfColorsMissing()
        {
            if (!ContainsColors)
            {
                throw new InvalidOperationException($"Mesh `{value}` does not contain colors");
            }
        }

        [Conditional("DEBUG")]
        private readonly void ThrowIfColorsPresent()
        {
            if (ContainsColors)
            {
                throw new InvalidOperationException($"Mesh `{value}` already contains colors");
            }
        }
    }
}